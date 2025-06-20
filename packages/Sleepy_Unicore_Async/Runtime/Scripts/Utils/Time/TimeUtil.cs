using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.Events;

namespace Sleepy.Async
{
    public static class TimeUtil
    {
        #region Timeout

        /// <summary>
        /// Sleepy 包装后的 UniTask 超时功能，在超时时抛出特定的异常。<br/>
        /// Sleepy wrapped timeout feature of UniTask, throwing a specific exception on timeout.
        /// </summary>
        /// <typeparam name="T">UniTask 返回的类型。 / The type returned by the UniTask.</typeparam>
        /// <param name="task">要添加超时功能的 UniTask。 / The UniTask to add timeout functionality to.</param>
        /// <param name="seconds">超时的秒数。 / The number of seconds before timeout.</param>
        /// <param name="delayType">延迟类型（默认为 DeltaTime）。 / The type of delay (default is DeltaTime).</param>
        /// <param name="timeoutCheckTiming">超时检查的时机（默认为 Update）。 / The timing for timeout checks (default is Update).</param>
        /// <param name="taskCancellationTokenSource">任务的取消令牌源（可选）。 / The cancellation token source for the task (optional).</param>
        /// <returns>任务的结果。 / The result of the task.</returns>
        /// <exception cref="SleepyTimeoutException">当任务超时时抛出。 / Thrown when the task times out.</exception>
        public static async UniTask<T> SleepyTimeout<T>(this UniTask<T> task, int seconds, DelayType delayType = DelayType.DeltaTime, PlayerLoopTiming timeoutCheckTiming = PlayerLoopTiming.Update, System.Threading.CancellationTokenSource taskCancellationTokenSource = null)
        {
            try
            {
                // Use UniTask's nice Timeout function here.
                return await task.Timeout(TimeSpan.FromSeconds(seconds),
                    delayType, timeoutCheckTiming, taskCancellationTokenSource);
            }
            catch (TimeoutException)
            {
                throw new SleepyTimeoutException(seconds);
            }
        }

        #endregion

        #region Countdown

        /// <summary>
        /// 启动一个新的倒计时，并返回其控制器。<br/>
        /// Starts a new countdown and returns its controller.
        /// </summary>
        /// <param name="seconds">倒计时的总秒数。 / The total number of seconds for the countdown.</param>
        /// <param name="tickCallback">倒计时每秒触发的回调。 / The callback triggered every second of the countdown.</param>
        /// <param name="finishCallback">倒计时完成时触发的回调。 / The callback triggered when the countdown finishes.</param>
        /// <param name="useTimeScale">这个计时器是否随Unity TimeScale改变。/ Is this countdown affected by the Unity TimeScale</param>
        /// <returns>新创建的倒计时控制器。 / The newly created countdown controller.</returns>
        public static CountdownController StartCountdown(int seconds, UnityAction<int> tickCallback = null, UnityAction finishCallback = null, bool useTimeScale = false)
        {
            return CountdownManager.StartCountdown(seconds, tickCallback, finishCallback, useTimeScale);
        }

        /// <summary>
        /// 快速重复利用一个 CountdownController，
        /// 如果是第一次启动则直接按照参数设置新建一个计时器；
        /// 如果这个 Countdown 已经启动过并且还未停止则直接重启倒计时；
        /// 如果 Countdown 已经停止了就新建一个同样参数的倒计时。<br/>
        /// Efficiently reuses a CountdownController, 
        /// directly creating a new timer with the specified parameters if it's the first start; 
        /// restarts the countdown if it has already started and not yet stopped; 
        /// or creates a new countdown with the same parameters if the Countdown has stopped.
        /// </summary>
        /// <param name="seconds">倒计时的总秒数。 / The total number of seconds for the countdown.</param>
        /// <param name="countdownController">引用倒计时控制器。 / The reference to the countdown controller.</param>
        /// <param name="tickCallback">倒计时每秒触发的回调。 / The callback triggered every second of the countdown.</param>
        /// <param name="finishCallback">倒计时完成时触发的回调。 / The callback triggered when the countdown finishes.</param>
        /// <param name="useTimeScale">这个计时器是否随Unity TimeScale改变。/ Is this countdown affected by the Unity TimeScale</param>
        public static void StartOrRestartCountdown(int seconds, ref CountdownController countdownController, UnityAction<int> tickCallback = null, UnityAction finishCallback = null, bool useTimeScale = false)
        {
            if (countdownController != null)
            {
                if (!countdownController.Restart())
                {
                    countdownController = StartCountdown(seconds, tickCallback, finishCallback, useTimeScale);
                }
            }
            else
            {
                countdownController = StartCountdown(seconds, tickCallback, finishCallback, useTimeScale);
            }
        }

        /// <summary>
        /// 当你不想创建和维护一个新的倒计时实例时使用此方法。但请注意，同一时间只能有一个全局倒计时计时器。<br/>
        /// Use this method when you are lazy to create and maintain a new countdown instance. However, do note that there can only be one global countdown timer at the same time.
        /// </summary>
        /// <param name="seconds">倒计时的总秒数。 / The total number of seconds for the countdown.</param>
        /// <param name="onTick">每秒倒计时时触发的回调函数。 / Callback function triggered for each second of the countdown.</param>
        /// <param name="onCompleted">倒计时完成时的回调函数。 / Callback function for when the countdown is completed.</param>
        /// <param name="useTimeScale">这个计时器是否随Unity TimeScale改变。/ Is this countdown affected by the Unity TimeScale</param>
        public static void StartGlobalCountdown(int seconds, UnityAction<int> onTick = null, UnityAction onCompleted = null, bool useTimeScale = false)
        {
            CountdownManager.StartGlobalCountdown(seconds, onTick, onCompleted, useTimeScale);
        }

        /// <summary>
        /// 全局倒计时是否在执行中<br/>
        /// Check if there is a running global countdown
        /// </summary>
        /// <returns></returns>
        public static bool IsGlobalCountdownRunning => CountdownManager.IsGlobalCountdownRunning;

        /// <summary>
        /// 暂停全局倒计时。<br/>
        /// Pauses the global countdown.
        /// </summary>
        public static void PauseGlobalCountdown()
        {
            if (CountdownManager.GlobalCountdown == null)
            {
                Dev.Error("There is no Global Countdown Timer yet");
                return;
            }
            CountdownManager.GlobalCountdown.Pause();
        }

        /// <summary>
        /// 恢复全局倒计时。<br/>
        /// Resumes the global countdown.
        /// </summary>
        public static void ResumeGlobalCountdown()
        {
            if (CountdownManager.GlobalCountdown == null)
            {
                Dev.Error("There is no Global Countdown Timer yet");
                return;
            }
            CountdownManager.GlobalCountdown.Resume();
        }

        /// <summary>
        /// 重启全局倒计时，并返回是否成功。<br/>
        /// Restarts the global countdown and returns whether it was successful.
        /// </summary>
        /// <returns>是否成功重启倒计时。 / Whether the countdown was successfully restarted.</returns>
        public static bool RestartGlobalCountdown()
        {
            if (CountdownManager.GlobalCountdown == null)
            {
                Dev.Error("There is no Global Countdown Timer yet");
                return false;
            }
            return CountdownManager.GlobalCountdown.Restart();
        }

        /// <summary>
        /// 停止全局倒计时。<br/>
        /// Stops the global countdown. 
        /// </summary>
        public static void StopGlobalCountdown()
        {
            if (CountdownManager.GlobalCountdown == null)
            {
                Dev.Error("There is no Global Countdown Timer yet");
                return;
            }
            CountdownManager.GlobalCountdown.Stop();
        }

        /// <summary>
        /// 停止所有的倒计时计时器。<br/>
        /// Stops all countdown timers.
        /// </summary>
        public static void StopAllCountdowns()
        {
            CountdownManager.StopAllCountdowns();
        }

        #endregion

        #region Timer

        /// <summary>
        /// 快速启动或重启一个计时器，如果计时器已存在则重启，否则创建一个新的计时器。<br/>
        /// Starts or restarts a timer, restarting if the timer already exists or creating a new one otherwise.
        /// </summary>
        /// <param name="timerController">计时器控制器的引用。 / Reference to the timer controller.</param>
        /// <param name="tickCallback">计时器每次触发的回调。 / Callback for each tick of the timer.</param
        /// <param name="useTimeScale">这个计时器是否随Unity TimeScale改变。/ Is this timer affected by the Unity TimeScale</param>
        public static void StartTimer(ref TimerController timerController, UnityAction<int> tickCallback = null, bool useTimeScale = false)
        {
            if (timerController != null)
            {
                if (!timerController.Restart())
                {
                    timerController = StartTimer(tickCallback, useTimeScale);
                }
            }
            else
            {
                timerController = StartTimer(tickCallback);
            }
        }

        /// <summary>
        /// 启动一个新的计时器，并返回其控制器。<br/>
        /// Starts a new timer and returns its controller.
        /// </summary>
        /// <param name="tickCallback">计时器每次触发的回调。 / Callback for each tick of the timer.</param>
        /// <param name="useTimeScale">这个计时器是否随Unity TimeScale改变。/ Is this timer affected by the Unity TimeScale</param>
        /// <returns>新创建的计时器控制器。 / The newly created timer controller.</returns>
        public static TimerController StartTimer(UnityAction<int> tickCallback = null, bool useTimeScale = false)
        {
            return TimerManager.StartTimer(tickCallback, useTimeScale);
        }

        /// <summary>
        /// 启动全局计时器。<br/>
        /// Starts the global timer.
        /// </summary>
        /// <param name="onTick">计时器每次触发的回调。 / Callback for each tick of the timer.</param>
        /// <param name="useTimeScale">这个计时器是否随Unity TimeScale改变。/ Is this timer affected by the Unity TimeScale</param>
        public static void StartGlobalTimer(UnityAction<int> onTick = null, bool useTimeScale = false)
        {
            TimerManager.StartGlobalTimer(onTick, useTimeScale);
        }

        /// <summary>
        /// 全局计时是否在执行中<br/>
        /// Check if there is a running global timer
        /// </summary>
        /// <returns></returns>
        public static bool IsGlobalTimerRunning => TimerManager.IsGlobalTimerRunning;

        /// <summary>
        /// 暂停全局计时器。<br/>
        /// Pauses the global timer.
        /// </summary>
        public static void PauseGlobalTimer()
        {
            if (TimerManager.GlobalTimer == null)
            {
                Dev.Error("There is no Global Timer Timer yet");
                return;
            }
            TimerManager.GlobalTimer.Pause();
        }

        /// <summary>
        /// 恢复全局计时器。<br/>
        /// Resumes the global timer.
        /// </summary>
        public static void ResumeGlobalTimer()
        {
            if (TimerManager.GlobalTimer == null)
            {
                Dev.Error("There is no Global Timer Timer yet");
                return;
            }
            TimerManager.GlobalTimer.Resume();
        }

        /// <summary>
        /// 重启全局计时器。<br/>
        /// Restarts the global timer.
        /// </summary>
        public static void RestartGlobalTimer()
        {
            if (TimerManager.GlobalTimer == null)
            {
                Dev.Error("There is no Global Timer Timer yet");
                return;
            }
            TimerManager.GlobalTimer.Restart();
        }

        /// <summary>
        /// 停止全局计时器。<br/>
        /// Stops the global timer.
        /// </summary>
        public static void StopGlobalTimer()
        {
            if (TimerManager.GlobalTimer == null)
            {
                Dev.Error("There is no Global Timer Timer yet");
                return;
            }
            TimerManager.StopGlobalTimer();
        }

        /// <summary>
        /// 停止所有计时器。<br/>
        /// Stops all timers.
        /// </summary>
        public static void StopAllTimers()
        {
            TimerManager.StopAllTimers();
        }

        #endregion

        #region Others

        /// <summary>
        /// 将秒转换为毫秒。 / Converts seconds to milliseconds.
        /// </summary>
        /// <param name="seconds">要转换的秒数。比如 Time.time / The seconds to convert.</param>
        /// <returns>转换后的毫秒数。 / The converted milliseconds.</returns>
        public static int ToMilliseconds(float seconds)
        {
            return (int)(seconds * 1000f);
        }

        #endregion
    }
}