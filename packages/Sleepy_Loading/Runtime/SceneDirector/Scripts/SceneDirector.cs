using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sleepy.Loading
{
    /// <summary>
    /// 管理Unity场景的加载
    /// Manages asynchronous scene loading in Unity
    /// </summary>
    public static class SceneDirector
    {
        #region Common vars
        /// <summary>
        /// 存储当前激活的场景名称。<br/>
        /// Stores the names of the currently active scenes.
        /// </summary>
        private static HashSet<string> _activeScenes = new HashSet<string>(); // This is just for cache
        /// <summary>
        /// 获取当前激活的场景集合。<br/>
        /// Gets a collection of the currently active scenes.
        /// </summary>
        public static HashSet<string> ActiveScenes => CheckActiveScenes();
        /// <summary>
        /// 存储单场景加载操作<br/>
        /// Stores single-scene loading operations
        /// </summary>
        private static Dictionary<string, object> _loadingOperation = new Dictionary<string, object>();
        #endregion

        #region 单场景加载 / Single-Scene Loading

        /// <summary>
        /// 异步加载指定的单个场景，并报告进度。
        /// Asynchronously loads a specified single scene and reports the progress.
        /// </summary>
        /// <param name="sceneName">场景名称 / Scene name</param>
        /// <param name="progress">进度报告器 / Progress reporter</param>
        /// <param name="simulateDelay">模拟延迟 / Simulated delay</param>
        /// <returns>加载成功时返回 true / Returns true if loading is successful</returns>
        public static async UniTask<bool> LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Additive, IProgress<float> progress = null, float simulateDelay = 0f)
        {
            try
            {
                if (SceneManager.GetSceneByName(sceneName).isLoaded)
                {
                    Dev.Error($"{sceneName} is already in loaded");
                    return false;
                }

                if (_loadingOperation.ContainsKey(sceneName))
                {
                    Dev.Error($"{sceneName} is already in loading!");
                    return false;
                }

                // 开始加载场景
                // Start loading the scene
                if (IsSceneInBuildSettings(sceneName))
                {
                    var loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    loadOperation.allowSceneActivation = false;

                    _loadingOperation[sceneName] = loadOperation;

                    // 模拟延迟过程
                    // Simulating the delay process
                    while (simulateDelay > 0)
                    {
                        if (!_loadingOperation.ContainsKey(sceneName))
                        {
                            // 如果加载被取消
                            // If loading is canceled
                            return false;
                        }
                        simulateDelay -= Time.deltaTime;
                        await UniTask.Yield(PlayerLoopTiming.Update);
                    }

                    // 等待加载完成
                    // Wait for the loading to complete
                    while (loadOperation.progress < .9f)
                    {
                        if (!_loadingOperation.ContainsKey(sceneName))
                        {
                            // 如果加载被取消
                            // If loading is canceled
                            return false;
                        }
                        progress?.Report(loadOperation.progress);
                        await UniTask.Yield(PlayerLoopTiming.Update);
                    }

                    if (!_loadingOperation.ContainsKey(sceneName))
                    {
                        // 如果加载被取消
                        // If loading is canceled
                        return false;
                    }
                    // 加载完成后激活场景
                    // Activate the scene after loading completes
                    loadOperation.allowSceneActivation = true;
                    _loadingOperation.Remove(sceneName);
                    progress?.Report(1);
                }
                else
                {
                    Dev.Log($"Try to load scene {sceneName} from addressable...");
                    AddressableSceneLoadObject loadObject = new AddressableSceneLoadObject();
                    _loadingOperation[sceneName] = loadObject;
                    await AssetLoader.LoadAddressableScene(sceneName, progress, simulateDelay, loadObject);

                    _loadingOperation.Remove(sceneName);
                }

            }
            catch (SleepyException ex)
            {
                Debug.LogException(ex);
                _loadingOperation.Remove(sceneName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 取消指定场景的加载。
        /// Cancels the loading of a specified scene.
        /// </summary>
        /// <param name="sceneName">要取消加载的场景名称。
        /// The name of the scene to cancel loading.</param>
        public static async UniTask CancelLoading(string sceneName)
        {

            if (_loadingOperation.TryGetValue(sceneName, out object obj))
            {
                try
                {
                    AsyncOperation op = (AsyncOperation)obj;

                    _loadingOperation.Remove(sceneName);

                    // 立即异步卸载场景
                    // Immediately asynchronously unload the scene
                    await UnloadLoadingScene(op, sceneName);
                }
                catch (Exception)
                {
                    try
                    {
                        // 如果失败了可能是场景使用了 Addressable 加载
                        AddressableSceneLoadObject loadObject = (AddressableSceneLoadObject)obj;

                        AssetLoader.CancelAddressableSceneLoading(loadObject);

                        _loadingOperation.Remove(sceneName);
                    }
                    catch
                    {
                        Dev.Error($"Cannot get {sceneName} loading handler");
                    }
                }
            }
            else
            {
                Dev.Warning($"{sceneName} is not loading");
            }
        }

        #endregion

        #region Unload Scene(s)

        /// <summary>
        /// 异步卸载指定的场景。<br/>
        /// Asynchronously unloads a specified scene.
        /// </summary>
        /// <param name="sceneName">要卸载的场景名称。
        /// The name of the scene to be unloaded.</param>
        /// <returns>如果场景成功卸载，则返回 true；否则返回 false。
        /// Returns true if the scene is successfully unloaded, otherwise false.</returns>
        public static async UniTask<bool> UnloadSceneAsync(string sceneName)
        {
            try
            {
                // 检查场景是否正在加载中
                // Check if the scene is currently being loaded
                if (_loadingOperation.ContainsKey(sceneName))
                {
                    Dev.Log($"{sceneName} is in loading, stop and unload it");
                    await CancelLoading(sceneName);
                    return true;
                }

                // 检查场景是不是通过Addressable加载的
                // Check if the scene is loaded by addressable
                if (AssetLoader.IsSceneLoadedByAddressable(sceneName))
                {
                    return AssetLoader.UnloadAddressableSceneLoading(sceneName);
                }

                // 检查场景是否已加载
                // Check if the scene is loaded
                if (!SceneManager.GetSceneByName(sceneName).isLoaded)
                {
                    Dev.Error($"{sceneName} is not loaded.");
                    return false;
                }

                // 执行场景卸载操作
                // Perform the scene unloading operation
                await SceneManager.UnloadSceneAsync(sceneName);
                return true;
            }
            catch (SleepyException e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// 异步卸载多个指定的场景。<br/>
        /// Asynchronously unloads multiple specified scenes.
        /// </summary>
        /// <param name="sceneNames">要卸载的场景名称列表。
        /// List of scene names to be unloaded.</param>
        /// <returns>如果所有指定场景都成功卸载，则返回 true。
        /// Returns true if all specified scenes are successfully unloaded.</returns>
        public static async UniTask<bool> UnloadScenesAsync(List<string> sceneNames)
        {
            List<UniTask> unloadTasks = new List<UniTask>();
            foreach (var sceneName in sceneNames)
            {
                unloadTasks.Add(UnloadSceneAsync(sceneName));
            }

            try
            {
                // 等待所有场景卸载任务完成
                // Wait for all scene unload tasks to complete
                await UniTask.WhenAll(unloadTasks);
                return true;
            }
            catch (SleepyException e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// 卸载正在加载的场景。
        /// Unloads a scene that is loading.
        /// </summary>
        /// <param name="operation">正在进行的场景加载操作。
        /// The ongoing scene loading operation.</param>
        /// <param name="sceneName">要卸载的场景名称。
        /// The name of the scene to unload.</param>
        private static async UniTask UnloadLoadingScene(AsyncOperation operation, string sceneName)
        {
            // Unity 只能在场景被完全载入后才能卸载
            // Unity can only unload a scene after it's fully loaded
            await UniTask.WaitUntil(() => (operation == null) || (operation.progress >= .9f));
            if (operation != null) operation.allowSceneActivation = true;

            // 得等一下不然无法识别场景已加载
            // Must wait a bit to recognize the scene as loaded
            while (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            await SceneManager.UnloadSceneAsync(sceneName);
        }

        /// <summary>
        /// 检查并返回当前已加载的场景。<br/>
        /// Checks and returns the currently loaded scenes.
        /// </summary>
        /// <returns>包含已加载场景名称的 HashSet。
        /// A HashSet containing the names of the loaded scenes.</returns>
        private static HashSet<string> CheckActiveScenes()
        {
            _activeScenes.Clear();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                {
                    // 如果场景已加载，将其名称添加到集合中
                    // If the scene is loaded, add its name to the collection
                    _activeScenes.Add(scene.name);
                }
            }
            return _activeScenes;
        }
        #endregion

        #region PUBLIC UTILS

        /// <summary>
        /// 检查指定的场景是否已加载并处于活动状态。<br/>
        /// Checks if the specified scene is loaded and active.
        /// </summary>
        /// <param name="sceneName">要检查的场景名称。/ The name of the scene to check.</param>
        /// <returns>True if the scene is loaded and active, false otherwise.</returns>
        public static bool IsSceneActive(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                {
                    if (sceneName == scene.name) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查指定的场景是否包含在构建设置中。<br/>
        /// Checks if a specified scene is included in the build settings.
        /// </summary>
        /// <param name="sceneName">要检查的场景名称。/ The name of the scene to check.</param>
        /// <returns>如果场景包含在构建设置中则返回 true，否则返回 false。/ Returns true if the scene is included in the build settings, otherwise false.</returns>
        public static bool IsSceneInBuildSettings(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                if (!string.IsNullOrEmpty(path))
                {
                    string name = System.IO.Path.GetFileNameWithoutExtension(path);
                    if (sceneName == name) return true;
                }
            }
            return false;
        }
        #endregion
    }
}