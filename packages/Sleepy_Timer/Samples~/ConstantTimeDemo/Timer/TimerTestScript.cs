using System;
using Sleepy.Timer;
using TMPro;
using UniRx;
using UnityEngine;

namespace Sleepy.Demo.Timer
{
    internal class TimerTestScript : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _timer1UI;
        [SerializeField] TextMeshProUGUI _timer2UI;

        TimerController _timerController1;
        TimerController _timerController2;
        IDisposable _timer2disposable;

        public void RenderText(TextMeshProUGUI ui, int time, string desc)
        {
            ui.text = $"{desc}: {time.ToString()}";
        }

        public void StartTimer(int i)
        {
            switch (i)
            {
                case 1:
                    // Use the callbacks to control timer1
                    TimeUtil.StartTimer(ref _timerController1, val =>
                    {
                        RenderText(_timer1UI, val, "Timer1");
                    });
                    break;
                case 2:
                    // Use UniRx subscribe to control timer2
                    TimeUtil.StartTimer(ref _timerController2);
                    _timer2disposable = _timerController2.PassedTime.Subscribe(val =>
                    {
                        RenderText(_timer2UI, val, "Timer2");
                    }).AddTo(this);
                    break;
                default:
                    break;
            }
        }

        public void PauseTimer(int i)
        {
            switch (i)
            {
                case 1:
                    if (_timerController1 != null)
                    {
                        _timerController1.Pause();
                    }
                    break;
                case 2:
                    if (_timerController2 != null)
                    {
                        _timerController2.Pause();
                    }
                    break;
                default:
                    break;
            }
        }

        public void ResumeTimer(int i)
        {
            switch (i)
            {
                case 1:
                    if (_timerController1 != null)
                    {
                        _timerController1.Resume();
                    }
                    break;
                case 2:
                    if (_timerController2 != null)
                    {
                        _timerController2.Resume();
                    }
                    break;
                default:
                    break;
            }
        }

        public void RestartTimer(int i)
        {
            switch (i)
            {
                case 1:
                    if (_timerController1 != null)
                    {
                        _timerController1.Restart();
                    }
                    break;
                case 2:
                    if (_timerController2 != null)
                    {
                        _timerController2.Restart();
                    }
                    break;
                default:
                    break;
            }
        }

        public void StopTimer(int i)
        {
            switch (i)
            {
                case 1:
                    if (_timerController1 != null)
                    {
                        _timerController1.Stop();
                    }
                    break;
                case 2:
                    if (_timerController2 != null)
                    {
                        _timerController2.Stop();
                        _timer2disposable.Dispose();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}