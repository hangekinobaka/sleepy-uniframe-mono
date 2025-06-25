using System;
using Cysharp.Threading.Tasks;
using Sleepy.Loading;
using Sleepy.Timer;
using TMPro;
using UniRx;
using UnityEngine;

namespace Sleepy.Demo.Timer
{
    internal class TimeoutDemoController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _countdownTextUI;

        public async void LoadWithTimeout()
        {
            Progress<float> progress = new Progress<float>();

            // start timer
            CountdownController controller = TimeUtil.StartCountdown(7);

            controller.RemainingTime.Subscribe(val =>
            {
                _countdownTextUI.text = val.ToString();
            }).AddTo(this);

            try
            {
                await SceneDirector.LoadSceneAsync("FakeScene", UnityEngine.SceneManagement.LoadSceneMode.Additive, progress, 10).SleepyTimeout(7);
            }
            catch (SleepyTimeoutException e)
            {
                Debug.LogException(e);

                SceneDirector.CancelLoading("FakeScene").Forget();

                controller.Stop();
                _countdownTextUI.text = "Timeout!";
            }
        }
    }
}