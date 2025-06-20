using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Sleepy.Loading
{
    public static class AssetLoader
    {
        #region Handle Map

        static Dictionary<object, AsyncOperationHandle> _assetHandleMap = new Dictionary<object, AsyncOperationHandle>();

        #endregion

        #region Load

        /// <summary>
        /// 创建一个可寻址资源的异步操作句柄，并在完成时执行一个操作。<br/>
        /// Creates an async operation handle for an addressable asset and executes an action upon completion. Optionally releases the handle after completion.
        /// </summary>
        /// <param name="addressableKey">可寻址资源的键名。/ The key for the addressable asset.</param>
        /// <param name="onCompleteAction">加载完成时执行的回调函数。/ The callback action to execute on load completion.</param>
        /// <returns>可寻址资源的异步操作句柄。/ The async operation handle for the addressable asset.</returns>
        public static AsyncOperationHandle<T> CreateAddressableAssetHandle<T>(object addressableKey, Action<T> onCompleteAction = null) where T : UnityEngine.Object
        {
            AsyncOperationHandle<T> handle;
            // 检查资源是否已在缓存中 / Check if the asset is already in the cache
            if (_assetHandleMap.ContainsKey(addressableKey))
            {
                try
                {
                    // 尝试从缓存中获取资源 / Attempt to retrieve the asset from the cache
                    handle = _assetHandleMap[addressableKey].Convert<T>();
                }
                catch (Exception)
                {
                    // 缓存中的资源句柄出错，重新加载 / Cached asset handle is wrong, reload it
                    Dev.Warning($"Something wrong with your cached {addressableKey} handle.");
                    handle = Addressables.LoadAssetAsync<T>(addressableKey);
                    _assetHandleMap[addressableKey] = handle;
                }
            }
            else
            {
                // 开始异步加载指定的 Addressable 资源 / Start loading the specified Addressable asset asynchronously
                handle = Addressables.LoadAssetAsync<T>(addressableKey);
                _assetHandleMap.Add(addressableKey, handle);
            }

            handle.Completed += (operationHandle) =>
            {
                if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    // 获取加载的资源 / Retrieve the loaded asset
                    T result = operationHandle.Result;
                    onCompleteAction?.Invoke(result);
                }
                else
                {
                    // 处理加载失败的情况 / Handle the load failure case
                    Dev.Error($"Something wrong when loading asset {addressableKey}");
                }

            };

            return handle;
        }

        // 加载指定 Addressable 键名的资源句柄 / Load the resource handle for the specified Addressable key
        private static async UniTask<AsyncOperationHandle<T>> LoadAddressableAssetHandleInternal<T>(object addressableKey, IProgress<float> progress = null) where T : UnityEngine.Object
        {
            AsyncOperationHandle<T> handle = CreateAddressableAssetHandle<T>(addressableKey);

            // 如果提供了进度回调，则报告加载进度 / If a progress callback is provided, report the loading progress
            if (progress != null)
            {
                // 循环等待直到资源加载完成 / Loop until the asset has finished loading
                while (!handle.IsDone)
                {
                    await UniTask.Yield();
                    // 报告加载进度 / Report the loading progress
                    progress?.Report(handle.PercentComplete);
                }
            }
            else
            {
                // 等待异步操作完成 / Wait for the asynchronous operation to complete
                await handle.ToUniTask();
            }

            // 检查加载操作的状态，记录错误 / Check the status of the loading operation and log errors
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Dev.Error($"Something wrong when loading asset {addressableKey}");
            }

            return handle;
        }

        /// <summary>
        /// 异步加载 Addressable 资源并返回结果。<br/>
        /// Asynchronously loads an Addressable asset and returns the result.
        /// </summary>
        /// <param name="addressableKey">要加载的 Addressable 资源键名。<br/>
        /// The key of the Addressable asset to load.</param>
        /// <param name="progress">加载进度回调。<br/>
        /// Callback for the loading progress.</param>
        /// <returns>加载的 Addressable 资源，加载失败则为 null。<br/>
        /// The loaded Addressable asset, or null if the loading fails.</returns>
        public static async UniTask<T> LoadAddressableAsset<T>(object addressableKey, IProgress<float> progress = null) where T : UnityEngine.Object
        {
            // 加载资源句柄 / Load the resource handle
            AsyncOperationHandle<T> handle = await LoadAddressableAssetHandleInternal<T>(addressableKey, progress);

            try
            {
                // 尝试获取加载的资源结果 / Attempt to get the loaded asset result
                T result = handle.Result;

                // 如果结果非空，则返回结果 / If the result is not null, return the result
                if (result != null)
                {
                    return result;
                }
                else
                {
                    throw new SleepyException("Addressable is loaded as null");
                }
            }
            catch (Exception e)
            {
                // 加载失败时记录错误 / Log the error when the load fails
                Dev.Error($"Load Addressable - {addressableKey} - failed: {e.Message}");

                // 检查并释放有效的资源句柄 / Check and release a valid resource handle
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
                _assetHandleMap.Remove(addressableKey);

                return null;
            }
        }

        /// <summary>
        /// 获取缓存中的 Addressable 资源，如果存在。<br/>
        /// Gets the Addressable asset from the cache, if it exists.
        /// </summary>
        /// <param name="addressableKey">要获取的 Addressable 资源的键名。<br/>
        /// The key of the Addressable asset to get.</param>
        /// <returns>获取的 Addressable 资源，如果未找到则为默认值。<br/>
        /// The retrieved Addressable asset, or the default value if not found.</returns>
        public static T GetAddressableAsset<T>(object addressableKey) where T : UnityEngine.Object
        {
            T result = default(T);
            // 检查资源是否已在缓存中 / Check if the asset is already in the cache
            if (_assetHandleMap.ContainsKey(addressableKey))
            {
                try
                {
                    // 尝试从缓存中获取资源 / Attempt to retrieve the asset from the cache
                    AsyncOperationHandle<T> handle = _assetHandleMap[addressableKey].Convert<T>();
                    result = handle.Result;

                }
                catch (Exception)
                {
                    // 缓存中的资源句柄出错，记录错误并移除缓存条目 / Cached asset handle is wrong, log error and remove cache entry
                    Dev.Error($"Something wrong with your cached {addressableKey} handle.");
                    _assetHandleMap.Remove(addressableKey);
                }
            }
            else
            {
                // 未找到缓存时记录错误 / Log error if cache is not found
                Dev.Error($"There is no cache for {addressableKey}.");
            }

            return result;
        }

        /// <summary>
        /// 释放指定的可寻址资源句柄。<br/>
        /// Releases the specified Addressable asset handle.
        /// </summary>
        /// <param name="handle">要释放的资源句柄。/ The handle of the asset to release.</param>
        public static void ReleaseAddressableHandle(AsyncOperationHandle handle)
        {
            // 检查句柄是否有效 / Check if the handle is valid
            if (handle.IsValid())
            {
                // 释放资源句柄 / Release the resource handle
                Addressables.Release(handle);
            }

            if (_assetHandleMap.ContainsValue(handle))
            {
                object key = _assetHandleMap.FirstOrDefault(pair => pair.Value.Equals(handle)).Key;
                if (key != null) _assetHandleMap.Remove(key);
            }
            else
            {
                // 如果出于某种原因句柄不在缓存中，则发出警告
                // Emit a warning if the handle is not in the cache for some reason
                Dev.Warning($"{handle} is not in the cache for some reason.");
            }
        }

        /// <summary>
        /// 释放指定键的可寻址资源句柄。<br/>
        /// Releases the Addressable asset handle for the specified key.
        /// </summary>
        /// <param name="addressableKey">资源的键名。/ The key of the asset.</param>
        public static void ReleaseAddressableHandle(object addressableKey)
        {
            // 检查资源是否已加载 / Check if the asset is loaded
            if (_assetHandleMap.ContainsKey(addressableKey))
            {
                var handle = _assetHandleMap[addressableKey];
                // 检查句柄是否有效 / Check if the handle is valid
                if (handle.IsValid())
                {
                    // 释放资源句柄 / Release the resource handle
                    Addressables.Release(handle);
                }
                _assetHandleMap.Remove(addressableKey);
            }
            else
            {
                // 资源未加载时发出警告 / Emit warning if the asset is not loaded
                Dev.Warning($"{addressableKey} is not loaded.");
            }
        }

        #endregion

        #region Multi-Loading

        /// <summary>
        /// 创建多个 Addressable 资源的句柄。<br/>
        /// Creates handles for multiple Addressable assets.
        /// </summary>
        /// <param name="addressableKeys">Addressable 资源键名的数组。<br/>
        /// Array of Addressable asset keys.</param>
        /// <returns>Addressable 资源的操作句柄数组。<br/>
        /// Array of operation handles for the Addressable assets.</returns>
        public static AsyncOperationHandle[] CreateMultipleAddressableAssetHandle<T>(string[] addressableKeys) where T : UnityEngine.Object
        {
            AsyncOperationHandle[] handles = new AsyncOperationHandle[addressableKeys.Length];
            for (int i = 0; i < addressableKeys.Length; i++)
            {
                handles[i] = CreateAddressableAssetHandle<T>(addressableKeys[i]);
            }
            return handles;
        }

        /// <summary>
        /// 等待多个 Addressable 资源句柄完成加载。<br/>
        /// Waits for multiple Addressable asset handles to complete loading.
        /// </summary>
        /// <param name="handles">Addressable 资源的操作句柄数组。<br/>
        /// Array of operation handles for the Addressable assets.</param>
        /// <param name="progress">加载进度回调。<br/>
        /// Callback for the loading progress.</param>
        /// <returns>如果所有 Addressable 资源成功加载，则返回 true；否则返回 false。<br/>
        /// Returns true if all Addressable assets are successfully loaded, false otherwise.</returns>
        public static async UniTask<bool> WaitForMultipleAddressableAssetHandles(AsyncOperationHandle[] handles, IProgress<float> progress = null)
        {
            if (progress != null)
            {
                // 每帧检查进度直到所有操作完成 / Check progress each frame until all operations are complete
                while (handles.Any(handle => !handle.IsDone))
                {
                    // 报告总体进度 / Report overall progress
                    progress.Report(handles.Average(handle => handle.PercentComplete));
                    await UniTask.Yield();
                }
            }
            else
            {
                var tasks = handles.Select(handle => handle.ToUniTask()).ToList();
                // 等待所有任务完成 / Wait for all tasks to complete
                await UniTask.WhenAll(tasks);
            }

            // 检查所有操作是否成功完成 / Check if all operations are successfully completed
            if (!handles.All(handle => handle.Status == AsyncOperationStatus.Succeeded))
            {
                Dev.Error("Addressables loading error.");
                return false;
            }
            return true;
        }


        /// <summary>
        /// 释放多个 Addressable 资源句柄。<br/>
        /// Releases multiple Addressable asset handles.
        /// </summary>
        /// <param name="addressableKeys">要释放的 Addressable 资源键名数组。<br/>
        /// Array of the Addressable asset keys to release.</param>
        public static void ReleaseMultipleAddressableAsset(string[] addressableKeys)
        {
            foreach (string key in addressableKeys)
            {
                ReleaseAddressableHandle(key);
            }
        }

        /// <summary>
        /// 释放多个可寻址资源句柄。<br/>
        /// Releases multiple Addressable asset handles.
        /// </summary>
        /// <param name="handles">要释放的资源句柄数组。/ The array of handles to be released.</param>
        public static void ReleaseMultipleAddressableAsset(AsyncOperationHandle[] handles)
        {
            foreach (var handle in handles)
            {
                ReleaseAddressableHandle(handle);
            }
        }


        #endregion

        #region Preload

        /// <summary>
        /// 预加载指定的 Addressable 资源。<br/>
        /// Preloads the specified Addressable asset.
        /// </summary>
        /// <param name="addressableKey">要预加载的 Addressable 资源的键名。<br/>
        /// The key of the Addressable asset to preload.</param>
        public static async UniTask PreloadAddressableAsset(object addressableKey)
        {
            // 检查资源是否已在缓存中 / Check if the asset is already in the cache
            if (_assetHandleMap.ContainsKey(addressableKey)) return;

            AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync(addressableKey);

            try
            {
                await handle.Task; // 等待预加载完成 / Wait for the preload to complete
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    // 预加载成功日志记录 / Log success of preload
                    Dev.Log($"Preloaded Addressable asset: {addressableKey}");
                }
                else
                {
                    // 预加载失败，抛出异常 / Preload failed, throw exception
                    throw new SleepyException($"Failed to preload Addressable asset: {addressableKey}");
                }
            }
            catch (Exception e)
            {
                // 预加载异常日志记录 / Log exception during preload
                Dev.Error($"Preload Addressable asset - {addressableKey} - failed: {e.Message}");
            }
        }

        #endregion

        #region Check Status

        /// <summary>
        /// 异步检查指定名称的Addressable资产是否存在。<br/>
        /// Asynchronously checks if an Addressable asset with the specified name exists.
        /// </summary>
        /// <param name="name">要检查的资产名称。/ The name of the asset to check.</param>
        /// <returns>如果资产存在，则为true；否则为false。/ True if the asset exists, false otherwise.</returns>
        public static async UniTask<bool> IsAddressableAsset(string name)
        {
            var locations = await Addressables.LoadResourceLocationsAsync(name).Task;
            return locations.Count > 0;
        }

        #endregion

        #region Scene Related

        /// <summary>
        /// 异步加载地址化场景，并提供进度反馈和可选的加载延迟模拟。<br/>
        /// Asynchronously loads an Addressable scene, providing progress feedback and an optional load delay simulation.
        /// </summary>
        /// <param name="sceneName">要加载的场景名称。<br/>
        /// The name of the scene to load.</param>
        /// <param name="progress">用于报告加载进度的进度指示器，如果为 null，则不报告。<br/>
        /// The progress indicator for reporting load progress, or null to report none.</param>
        /// <param name="simulateDelay">模拟加载过程中的延迟时间（秒）。<br/>
        /// The time in seconds to simulate a delay during the load process.</param>
        /// <param name="loadObject">场景加载的可选状态对象，用于管理加载过程。<br/>
        /// An optional state object for the scene loading, used to manage the loading process.</param>
        /// <returns>如果场景成功加载，则返回 true；如果加载失败或被取消，则返回 false。<br/>
        /// Returns true if the scene is successfully loaded, or false if the loading fails or is cancelled.</returns>
        internal static async UniTask<bool> LoadAddressableScene(string sceneName, IProgress<float> progress = null, float simulateDelay = 0f, AddressableSceneLoadObject loadObject = null)
        {
            if (loadObject == null)
            {
                loadObject = new AddressableSceneLoadObject();
            }
            loadObject.SceneName = sceneName;

            // 不自动激活场景，等待我们手动激活
            // Do not auto-activate the scene, wait for manual activation
            AsyncOperationHandle handle = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive, false);
            loadObject.Handle = handle;
            // Cache the handle
            _assetHandleMap.Add(sceneName, handle);

            // 即使被取消了我们也等待它读取完毕，不然无法unload
            // Even if canceled, we wait for it to finish loading so we can unload

            // 模拟延迟过程
            // Simulate the delay process
            while (handle.IsValid() && !loadObject.IsCanceled && simulateDelay > 0)
            {
                simulateDelay -= Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            // 等待加载完成
            // Wait for the loading to complete
            while (handle.IsValid() && !handle.IsDone)
            {
                progress?.Report(handle.PercentComplete);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            // Must wait a bit to recognize the scene as loaded
            if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded)
            {
                SceneInstance sceneInstance = (SceneInstance)handle.Result;

                // Now you can activate it
                await sceneInstance.ActivateAsync().ToUniTask();

                // If already canceled, just unload it
                if (loadObject.IsCanceled)
                {
                    UnloadAddressableSceneLoadingInternal(handle);
                }
            }
            else
            {
                Dev.Warning($"Something went wrong when loading scene {sceneName}.(Might be canceled)");
                return false;
            }

            progress?.Report(1);
            return true;
        }

        /// <summary>
        /// 卸载由Addressable加载的场景。<br/>
        /// Unloads a scene loaded by Addressables.
        /// </summary>
        /// <param name="handle">场景加载的操作句柄。/ The operation handle for the loaded scene.</param>
        private static bool UnloadAddressableSceneLoadingInternal(AsyncOperationHandle handle)
        {
            if (!handle.IsValid()) return false;
            SceneInstance sceneInstance = (SceneInstance)handle.Result;
            _assetHandleMap.Remove(sceneInstance.Scene.name);
            Addressables.UnloadSceneAsync(handle);
            return true;
        }

        /// <summary>
        /// 通过场景名称卸载由Addressable加载的场景。<br/>
        /// Unloads an Addressables-loaded scene by its name.
        /// </summary>
        /// <param name="sceneName">场景的名称。/ The name of the scene.</param>
        internal static bool UnloadAddressableSceneLoading(string sceneName)
        {
            if (!_assetHandleMap.ContainsKey(sceneName))
            {
                Dev.Error($"Cannot find the handle for {sceneName}");
                return false;
            }
            UnloadAddressableSceneLoadingInternal(_assetHandleMap[sceneName]);
            return true;
        }

        /// <summary>
        /// 检查指定场景是否已经由Addressable加载。<br/>
        /// Checks if the specified scene has been loaded by Addressable.
        /// </summary>
        /// <param name="sceneName">要检查的场景名称。/ The name of the scene to check.</param>
        /// <returns>如果场景已加载则返回true，否则返回false。/ Returns true if the scene is loaded, false otherwise.</returns>
        internal static bool IsSceneLoadedByAddressable(string sceneName)
        {
            return _assetHandleMap.ContainsKey(sceneName);
        }


        /// <summary>
        /// 取消Addressable场景的加载。<br/>
        /// Cancels the loading of an Addressable scene.
        /// </summary>
        /// <param name="loadObject">场景加载的辅助对象。/ The helper object for scene loading.</param>
        internal static void CancelAddressableSceneLoading(AddressableSceneLoadObject loadObject)
        {
            loadObject.IsCanceled = true;
        }

        #endregion
    }

}
