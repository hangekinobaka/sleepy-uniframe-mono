using Sleepy.Timer;
using TMPro;
using UnityEngine;

namespace Sleepy.Demo.Timer
{
    internal class GameTimeController : MonoBehaviour
    {
        [SerializeField] Animator _animDemo;

        [SerializeField] TextMeshProUGUI _countdownText;
        [SerializeField] TextMeshProUGUI _timerText;

        private void Awake()
        {
            _animDemo.SetTrigger("run");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Time.timeScale = 0.1f;
                Debug.Log("Time Slowdown!");
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                Time.timeScale = 10f;
                Debug.Log("Time Speedup!");
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Time.timeScale = 1f;
                Debug.Log("Time normal.");
            }
        }

        public void ToggleGameTimeCountdown()
        {
            if (TimeUtil.IsGlobalCountdownRunning)
            {
                TimeUtil.StopGlobalCountdown();
            }
            else
            {
                TimeUtil.StartGlobalCountdown(100, val => _countdownText.text = $"Countdown {val}", null, true); ;
            }
        }

        public void ToggleConstantTimeCountdown()
        {
            if (TimeUtil.IsGlobalCountdownRunning)
            {
                TimeUtil.StopGlobalCountdown();
            }
            else
            {
                TimeUtil.StartGlobalCountdown(100, val => _countdownText.text = $"Countdown {val}", null);
            }
        }
        public void ToggleGameTimeTimer()
        {
            if (TimeUtil.IsGlobalTimerRunning)
            {
                TimeUtil.StopGlobalTimer();
            }
            else
            {
                TimeUtil.StartGlobalTimer(val => _timerText.text = $"Timer {val}", true); ;
            }
        }

        public void ToggleConstantTimeTimer()
        {
            if (TimeUtil.IsGlobalTimerRunning)
            {
                TimeUtil.StopGlobalTimer();
            }
            else
            {
                TimeUtil.StartGlobalTimer(val => _timerText.text = $"Timer {val}");
            }
        }
    }
}
