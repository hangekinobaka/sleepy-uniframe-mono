using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sleepy.Async;
using Sleepy.Loading;
using TMPro;
using UniRx;
using UnityEngine;

namespace Sleepy.Demo.Loading
{
    // NOTE: To test this you need to add "DemoScene1", "DemoScene2" to the Build Setting
    // Make  "DemoScene3"  an addressable scene

    /// <summary>
    /// 用于测试 SceneDirector 功能的脚本。
    /// A script for testing SceneDirector functionalities.
    /// </summary>
    internal class SceneLoadingDemo : MonoBehaviour
    {
        [SerializeField] float _delay = 8f;
        [SerializeField] TextMeshProUGUI _countdownTextUI;

        /// <summary>
        /// 加载单个场景。
        /// Loads a single scene.
        /// </summary>
        public void LoadingScene(string sceneName)
        {
            Progress<float> progress = new Progress<float>(ReportProgress);

            // 开始加载一个场景，并传入进度接口和延迟时间。
            // Starts loading a scene, passing in the progress interface and delay time.
            SceneDirector.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive, progress, _delay).Forget();
        }

        /// <summary>
        /// 取消加载指定场景。
        /// Cancels loading a specific scene.
        /// </summary>
        public void CancelLoadingScene(string sceneName)
        {
            SceneDirector.CancelLoading(sceneName).Forget();
        }

        /// <summary>
        /// 报告场景加载进度。
        /// Reports the progress of scene loading.
        /// </summary>
        /// <param name="progressInfo">加载进度信息。
        /// Loading progress information.</param>
        private void ReportProgress(float progressInfo)
        {
            Debug.Log($"Loading Scene: {progressInfo * 100}%");
        }

        /// <summary>
        /// 获取并打印当前激活的所有场景名称。
        /// Gets and prints the names of all currently active scenes.
        /// </summary>
        public void GetCurrentScenes()
        {
            foreach (string name in SceneDirector.ActiveScenes)
            {
                Debug.Log(name);
            }
        }

        public void UnloadAll()
        {
            var scenesToUnload = new List<string> { "DemoScene1", "DemoScene2", "DemoScene3" };
            SceneDirector.UnloadScenesAsync(scenesToUnload).Forget();
        }

        public void UnloadScene(string sceneName)
        {
            SceneDirector.UnloadSceneAsync(sceneName).Forget();
        }

        public async void LoadWithTimeout()
        {
            Progress<float> progress = new Progress<float>(ReportProgress);

            // start timer
            CountdownController controller = TimeUtil.StartCountdown(7);

            controller.RemainingTime.Subscribe(val =>
            {
                _countdownTextUI.text = val.ToString();
            }).AddTo(this);

            try
            {
                await SceneDirector.LoadSceneAsync("DemoScene1", UnityEngine.SceneManagement.LoadSceneMode.Additive, progress, _delay).SleepyTimeout(7);
            }
            catch (SleepyTimeoutException e)
            {
                Debug.LogException(e);

                SceneDirector.CancelLoading("DemoScene1").Forget();

                controller.Stop();
                _countdownTextUI.text = "Timeout!";
            }
        }
    }
}