using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine.Events;

namespace Sleepy.Async
{
    /// <summary>
    /// 管理计时控制器的内部静态类。<br/>
    /// Internal static class for managing timer controllers.
    /// </summary>
    internal static class TimerManager
    {
        // 所有的计时控制器列表 / A list of all the timer controllers
        private static List<TimerController> _timerList = new List<TimerController>();

        // 整个app只能有一个全局计时器存在
        // There can only be one single global timer throughout the whole app.
        private static TimerController _globalTimer;
        internal static TimerController GlobalTimer => _globalTimer;

        /// <summary>
        /// 获取当前活跃的计时控制器数量。<br/>
        /// Gets the count of currently active timer controllers.
        /// </summary>
        public static int TotalTimerCounter => _timerList.Count;

        /// <summary>
        /// 全局计时是否在执行中<br/>
        /// Check if there is a running global timer
        /// </summary>
        /// <returns></returns>
        internal static bool IsGlobalTimerRunning => _globalTimer != null;

        /// <summary>
        /// 将计时控制器添加到管理器。<br/>
        /// Adds a timer controller to the manager.
        /// </summary>
        /// <param name="timer">要添加的计时控制器。<br/>
        /// The TimerController to be added to the list</param>
        internal static void AddTimer(TimerController timer)
        {
            // 如果列表中不包含此计时控制器，则添加它
            // If the list does not contain this timer controller, add it
            if (!_timerList.Contains(timer))
            {
                _timerList.Add(timer);
            }
        }

        /// <summary>
        /// 从管理器中移除指定的计时控制器。<br/>
        /// Removes a specified timer controller from the manager.
        /// </summary>
        /// <param name="timer">要移除的计时控制器。/ The TimerController to be removed from the list</param>
        internal static void RemoveTimer(TimerController timer)
        {
            // 如果列表中包含此计时控制器，则移除它
            // If the list contains this timer controller, remove it
            if (_timerList.Contains(timer))
            {
                _timerList.Remove(timer);
            }
        }

        /// <summary>
        /// 启动一个新的计时，并返回其控制器。<br/>
        /// Starts a new timer and returns its controller.
        /// </summary>
        /// <param name="onTick">每次计时的回调函数。 / callback for every second tick</param>
        /// <param name="useTimeScale">这个计时器是否随Unity TimeScale改变。/ Is this timer affected by the Unity TimeScale</param>
        /// <returns>新创建的计时控制器。 / The new TimerController that has been generated</returns>
        internal static TimerController StartTimer(UnityAction<int> onTick = null, bool useTimeScale = false)
        {
            return new TimerController(onTick, useTimeScale);
        }

        /// <summary>
        /// 当你不想创建和维护一个新的计时实例时使用此方法。但请注意，同一时间只能有一个全局计时器。<br/>
        /// Use this method when you are lazy to create and maintain a new timer instance. However, do note that there can only be one global timer at the same time.
        /// </summary>
        internal static void StartGlobalTimer(UnityAction<int> onTick = null, bool useTimeScale = false)
        {
            if (_globalTimer != null)
            {
                Dev.Log($"Global timer is interrupted.");
                StopGlobalTimer();
            }
            _globalTimer = StartTimer(onTick, useTimeScale);
        }

        /// <summary>
        /// 彻底停止全局计时器<br/>
        /// Stop the global timer completely
        /// </summary>
        internal static void StopGlobalTimer()
        {
            _globalTimer.Stop();
            _globalTimer = null;
        }

        /// <summary>
        /// 停止所有的计时器。<br/>
        /// Stops all timer timers.
        /// </summary>
        internal static void StopAllTimers()
        {
            List<TimerController> tempList = new List<TimerController>(_timerList);

            int i = 0;
            for (; i < tempList.Count; i++)
            {
                tempList[i].Stop();
            }
            Dev.Log($"Stopped {i} timer(s)");
        }
    }

    public class TimerController
    {
        // 缓存每次计时时触发的回调 / Cache the callback for each tick of the timer
        private UnityAction<int> _onTick;

        // 当前经过时间 / Time passed 
        private int _passedTime;
        /// <summary>
        /// You can subscribe this if you don't like the callback method
        /// </summary>
        public ReactiveProperty<int> PassedTime { get; } = new(0);

        // 用于控制计时订阅 / Used for controlling timer subscription
        private IDisposable _timerSubscription;
        // 记录是否暂停 / Tracks whether the timer is paused
        private bool _isPaused = false;
        // 记录是否已完成 / Tracks whether the timer is finished
        private bool _isCompleted = false;
        // 缓存是否遵循游戏时间设置 / Cache whether it follows the game time settings
        private bool _useTimeScale = false;

        /// <summary>
        /// Don't use this to create timer, use TimeUtil instead
        /// </summary>
        /// <param name="onTick">每次计时的回调。/ Callback for each tick of the timer.</param>
        /// <param name="useTimeScale">这个计时器是否随Unity TimeScale改变。/ Is this timer affected by the Unity TimeScale</param>
        internal TimerController(UnityAction<int> onTick = null, bool useTimeScale = false)
        {
            _onTick = onTick;
            _useTimeScale = useTimeScale;

            TimerManager.AddTimer(this);
            Dev.Log($"Start a new timer. There are {TimerManager.TotalTimerCounter} timer(s) in total.");

            StartTimerInternal(0);
        }

        // 内部启动计时的逻辑，使用了UniRx的订阅
        // Internal logic for starting the timer, use UniRx's subscribe to achieve the goal
        private void StartTimerInternal(int startTime = 0)
        {
            UpdateTime(startTime);
            _isCompleted = false;
            _isPaused = false;


            IScheduler scheduler = _useTimeScale ? Scheduler.MainThread : Scheduler.MainThreadIgnoreTimeScale;

            _timerSubscription = Observable.Interval(System.TimeSpan.FromSeconds(1), scheduler)
                .Subscribe(_ =>
                {
                    UpdateTime(_passedTime + 1); // 每次信号增加1秒 / Add 1 second every tick
                    _onTick?.Invoke(GetTime());
                });

        }

        private void UpdateTime(int time)
        {
            _passedTime = time;
            PassedTime.Value = time;
        }

        // 释放资源 / Dispose resources
        private void DisposeResources()
        {
            _isCompleted = true;

            TimerManager.RemoveTimer(this);
            Dev.Log($"Timer ended. There are {TimerManager.TotalTimerCounter} timer(s) in total.");

            _timerSubscription?.Dispose(); // 确保取消订阅 / Ensure subscription is disposed
            _timerSubscription = null;

            _onTick(0);
            _onTick = null;

            _useTimeScale = false;

            UpdateTime(0);
            _isPaused = false;
        }

        /// <summary>
        /// 获取当前经过时间。<br/>
        /// Gets the current passed time.
        /// </summary>
        public int GetTime()
        {
            return _passedTime;
        }

        /// <summary>
        /// 暂停计时。<br/>
        /// Pauses the timer.
        /// </summary>
        public void Pause()
        {
            if (_isPaused || _isCompleted)
            {
                Dev.Error("Cannot pause the timer. It is already paused or finished.");
                return;
            }

            _timerSubscription.Dispose(); // 取消订阅 / Dispose the subscription
            _timerSubscription = null;
            _isPaused = true;
            Dev.Log("Timer paused!");
        }

        /// <summary>
        /// 恢复计时。<br/>
        /// Resumes the timer.
        /// </summary>
        public void Resume()
        {
            if (!_isPaused || _isCompleted)
            {
                Dev.Error("Cannot Resume the timer. It is already started or finished.");
                return;
            }

            StartTimerInternal(_passedTime);
            Dev.Log("Timer resumed!");
        }

        /// <summary>
        /// 重启计时。<br/>
        /// Restarts the timer.
        /// </summary>
        public bool Restart()
        {
            if (_isCompleted)
            {
                Dev.Warning("Cannot Restart the timer. It is already started or finished.");
                return false;
            }

            if (_timerSubscription != null) _timerSubscription.Dispose(); // 取消订阅 / Dispose the subscription
            StartTimerInternal();
            Dev.Log("Timer restarted!");
            return true;
        }

        /// <summary>
        /// 停止并销毁计时。<br/>
        /// Stops and disposes of the timer.
        /// </summary>
        public void Stop()
        {
            if (_timerSubscription == null)
            {
                Dev.Error("Cannot Stop the timer. It is already finished.");
                return;
            }

            DisposeResources();
        }
    }
}