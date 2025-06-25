using Sleepy.Timer;
using TMPro;
using UnityEngine;

namespace Sleepy.Demo.Timer
{
    internal class CountdownTimerController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _countdownTextUI_10s;
        [SerializeField] TextMeshProUGUI _countdownTextUI_120s;
        [SerializeField] TextMeshProUGUI _countdownTextUI_global;

        CountdownController _10sController;
        CountdownController _120sController;

        void UpdateCountdownUI_10s(int time)
        {
            _countdownTextUI_10s.text = $"Countdown: {time}";
        }

        void CountdownFinshedHandler_10s()
        {
            _countdownTextUI_10s.text = $"Countdown: 0";
            Dev.Log("Countdown Finished!");
        }

        public void Start10()
        {
            TimeUtil.StartOrRestartCountdown(10, ref _10sController, UpdateCountdownUI_10s, CountdownFinshedHandler_10s);
        }
        public void Pause10()
        {
            _10sController.Pause();
        }
        public void Resume10()
        {
            _10sController.Resume();
        }
        public void Restart10()
        {
            _10sController.Restart();
        }
        public void Stop10()
        {
            _countdownTextUI_10s.text = $"Countdown: 0";
            _10sController.Stop();
        }

        void UpdateCountdownUI_120s(int time)
        {
            _countdownTextUI_120s.text = $"Countdown: {time}";
        }

        void CountdownFinshedHandler_120s()
        {
            _countdownTextUI_120s.text = $"Countdown: 0";
            Dev.Log("Countdown Finished!");
        }

        public void Start120()
        {
            TimeUtil.StartOrRestartCountdown(120, ref _120sController, UpdateCountdownUI_120s, CountdownFinshedHandler_120s);
        }
        public void Pause120()
        {
            _120sController.Pause();
        }
        public void Resume120()
        {
            _120sController.Resume();
        }
        public void Restart120()
        {
            _120sController.Restart();
        }
        public void Stop120()
        {
            _countdownTextUI_120s.text = $"Countdown: 0";
            _120sController.Stop();
        }

        void UpdateCountdownUI_global(int time)
        {
            _countdownTextUI_global.text = $"Global Countdown: {time}";
        }

        void CountdownFinshedHandler_global()
        {
            _countdownTextUI_global.text = $"Global Countdown: 0";
            Dev.Log("Countdown Finished!");
        }

        public void StartGlobal(int second)
        {
            TimeUtil.StartGlobalCountdown(second, UpdateCountdownUI_global, CountdownFinshedHandler_global);
        }
        public void PauseGlobal()
        {
            TimeUtil.PauseGlobalCountdown();
        }
        public void ResumeGlobal()
        {
            TimeUtil.ResumeGlobalCountdown();
        }
        public void RestartGlobal()
        {
            TimeUtil.RestartGlobalCountdown();
        }
        public void StopGlobal()
        {
            _countdownTextUI_global.text = $"Global Countdown: 0";
            TimeUtil.StopGlobalCountdown();
        }
        public void StopAll()
        {
            TimeUtil.StopAllCountdowns();
        }
    }
}