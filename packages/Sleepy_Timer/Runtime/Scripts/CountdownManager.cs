using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine.Events;

namespace Sleepy.Timer
{
    /// <summary>
    /// 管理倒计时控制器的内部静态类。<br/>
    /// Internal static class for managing countdown controllers.
    /// </summary>
    internal static class CountdownManager
    {
        // 所有的倒计时控制器列表 / A list of all the countdown controllers
        private static List<CountdownController> _countdownList = new List<CountdownController>();

        // 整个app只能有一个全局倒计时器存在
        // There can only be one single global countdown throughout the whole app.
        private static CountdownController _globalCountdown;
        internal static CountdownController GlobalCountdown => _globalCountdown;

        /// <summary>
        /// 获取当前活跃的倒计时控制器数量。<br/>
        /// Gets the count of currently active countdown controllers.
        /// </summary>
        internal static int TotalCountCounter => _countdownList.Count;

        /// <summary>
        /// 全局倒计时是否在执行中<br/>
        /// Check if there is a running global countdown
        /// </summary>
        /// <returns></returns>
        internal static bool IsGlobalCountdownRunning => _globalCountdown != null && !_globalCountdown.IsFinished;

        /// <summary>
        /// 将倒计时控制器添加到管理器。<br/>
        /// Adds a countdown controller to the manager.
        /// </summary>
        /// <param name="countdown">要添加的倒计时控制器。<br/>
        /// The CountdownController to be added to the list</param>
        internal static void AddCountdown(CountdownController countdown)
        {
            // 如果列表中不包含此倒计时控制器，则添加它
            // If the list does not contain this countdown controller, add it
            if (!_countdownList.Contains(countdown))
            {
                _countdownList.Add(countdown);
            }
        }

        /// <summary>
        /// 从管理器中移除指定的倒计时控制器。<br/>
        /// Removes a specified countdown controller from the manager.
        /// </summary>
        /// <param name="countdown">要移除的倒计时控制器。/ The CountdownController to be removed from the list</param>
        internal static void RemoveCountdown(CountdownController countdown)
        {
            // 如果列表中包含此倒计时控制器，则移除它
            // If the list contains this countdown controller, remove it
            if (_countdownList.Contains(countdown))
            {
                _countdownList.Remove(countdown);
            }
        }

        /// <summary>
        /// 启动一个新的倒计时，并返回其控制器。<br/>
        /// Starts a new countdown and returns its controller.
        /// </summary>
        /// <param name="seconds">倒计时的总秒数。/ The countdown total time</param>
        /// <param name="onTick">每次倒计时减少时的回调函数。 / callback for every second tick</param>
        /// <param name="onCompleted">倒计时完成时的回调函数。 / callback for timer finished </param>
        /// <param name="useTimeScale">这个计时器是否随Unity TimeScale改变。/ Is this countdown affected by the Unity TimeScale</param>
        /// <returns>新创建的倒计时控制器。 / The new CountdownController that has been generated</returns>
        internal static CountdownController StartCountdown(int seconds, UnityAction<int> onTick = null, UnityAction onCompleted = null, bool useTimeScale = false)
        {
            // 创建一个新的 CountdownController 实例
            // Creates a new CountdownController instance
            return new CountdownController(seconds, onTick, onCompleted, useTimeScale);
        }

        /// <summary>
        /// 当你不想创建和维护一个新的倒计时实例时使用此方法。但请注意，同一时间只能有一个全局倒计时计时器。
        /// Use this method when you are lazy to create and maintain a new countdown instance. However, do note that there can only be one global countdown at the same time.
        /// </summary>
        internal static void StartGlobalCountdown(int seconds, UnityAction<int> onTick = null, UnityAction onCompleted = null, bool useTimeScale = false)
        {
            if (_globalCountdown != null && !_globalCountdown.IsFinished)
            {
                Dev.Log($"Global countdown is interrupted.");
                _globalCountdown.Stop();
            }
            _globalCountdown = StartCountdown(seconds, onTick, onCompleted, useTimeScale);
        }

        /// <summary>
        /// 停止所有的倒计时计时器。<br/>
        /// Stops all countdowns.
        /// </summary>
        internal static void StopAllCountdowns()
        {
            List<CountdownController> tempList = new List<CountdownController>(_countdownList);

            int i = 0;
            for (; i < tempList.Count; i++)
            {
                tempList[i].Stop();
            }
            Dev.Log($"Stopped {i} countdown(s)");
        }
    }

    public class CountdownController
    {
        // 缓存初始设置的时间 / Cache the initial time set for the countdown
        private int _initTime;
        // 缓存每次计时减少时触发的回调 / Cache the callback for each tick of the countdown
        private UnityAction<int> _onTick;
        // 缓存计时结束时触发的回调 / Cache the callback for when the countdown completes
        private UnityAction _onCompleted;
        // 缓存是否遵循游戏时间设置
        private bool _useTimeScale = false;

        // 剩余时间 / Remaining time in the countdown
        private int _remainingTime;
        /// <summary>
        /// You can subscribe this if you don't like the callback method
        /// </summary>
        public ReactiveProperty<int> RemainingTime { get; } = new(0);
        // 用于控制倒计时订阅 / Used for controlling countdown subscription
        private IDisposable _countdownSubscription;
        // 记录是否暂停 / Tracks whether the countdown is paused
        private bool _isPaused = false;
        // 记录是否完成 / Tracks whether the countdown is finshed
        private bool _isCompleted = false;

        /// <summary>
        /// The countdown is already finished or not
        /// </summary>
        public bool IsFinished => !_isPaused && (_countdownSubscription == null);

        /// <summary>
        /// 创建一个新的倒计时控制器实例。<br/>
        /// Creates a new CountdownController instance.
        /// </summary>
        /// <param name="seconds">倒计时的总秒数。/ The total seconds for the countdown.</param>
        /// <param name="onTick">每次计时减少时的回调。/ Callback for each tick of the countdown.</param>
        /// <param name="onCompleted">计时结束时的回调。/ Callback when the countdown completes.</param>
        /// <param name="useTimeScale">这个计时器是否随Unity TimeScale改变。/ Is this countdown affected by the Unity TimeScale</param>
        internal CountdownController(int seconds, UnityAction<int> onTick = null, UnityAction onCompleted = null, bool useTimeScale = false)
        {
            _initTime = seconds;
            _onTick = onTick;
            _onCompleted = onCompleted;
            _useTimeScale = useTimeScale;

            CountdownManager.AddCountdown(this);
            Dev.Log($"Start a new {seconds}s countdown. There are {CountdownManager.TotalCountCounter} countdown(s) in total.");

            StartCountdownInternal(seconds);
        }

        // 内部启动倒计时的逻辑，使用了UniRx的订阅
        // Internal logic for starting the countdown, use UniRx's subscribe to achieve the goal
        private void StartCountdownInternal(int seconds)
        {
            UpdateRemainingTime(seconds);
            _isPaused = false;
            _isCompleted = false;

            IScheduler scheduler = _useTimeScale ? Scheduler.MainThread : Scheduler.MainThreadIgnoreTimeScale;

            _countdownSubscription = Observable.Timer(System.TimeSpan.Zero, System.TimeSpan.FromSeconds(1), scheduler)
                .Select(x => seconds - (int)x)
                .TakeWhile(x => x >= 0)
                .Subscribe(
                    x =>
                    {
                        _onTick?.Invoke(x);
                        UpdateRemainingTime(x);
                    },
                    () =>
                    {
                        _onCompleted?.Invoke();
                        DisposeResources();
                    }
                );
        }

        // 更新剩余时间 / Update remaining time
        private void UpdateRemainingTime(int time)
        {
            _remainingTime = time;
            RemainingTime.Value = time;
        }

        // 释放资源 / Dispose resources
        private void DisposeResources()
        {
            _isCompleted = true;

            CountdownManager.RemoveCountdown(this);
            Dev.Log($"Countdown ended. There are {CountdownManager.TotalCountCounter} countdown(s) in total.");

            _countdownSubscription?.Dispose(); // 确保取消订阅 / Ensure subscription is disposed
            _countdownSubscription = null;

            _initTime = 0;
            _isPaused = false;
            _onTick = null;
            _onCompleted = null;

            _useTimeScale = false;

            UpdateRemainingTime(0);
        }

        /// <summary>
        /// 获取剩余时间。<br/>
        /// Gets the remaining time.
        /// </summary>
        /// <returns>剩余时间（秒）。/ Remaining time in seconds.</returns>
        public int GetRemainingTime()
        {
            return _remainingTime;
        }

        /// <summary>
        /// 暂停倒计时。<br/>
        /// Pauses the countdown.
        /// </summary>
        public void Pause()
        {
            if (_isPaused || _isCompleted)
            {
                Dev.Error("Cannot pause the countdown. It is already paused or finished.");
                return;
            }

            _countdownSubscription.Dispose(); // 取消订阅 / Dispose the subscription
            _countdownSubscription = null;
            _isPaused = true;
            Dev.Log("Countdown paused!");
        }

        /// <summary>
        /// 恢复倒计时。<br/>
        /// Resumes the countdown.
        /// </summary>
        public void Resume()
        {
            if (!_isPaused || _isCompleted)
            {
                Dev.Error("Cannot Resume the countdown. It is already started or finished.");
                return;
            }

            StartCountdownInternal(_remainingTime);
            Dev.Log("Countdown resumed!");
        }

        /// <summary>
        /// 重启倒计时。<br/>
        /// Restarts the countdown.
        /// </summary>
        /// <returns>如果成功重启返回 true，否则返回 false。/ Returns true if successfully restarted, otherwise false.</returns>
        public bool Restart()
        {
            if (_isCompleted)
            {
                Dev.Warning("Cannot Restart the countdown. It is already finished.");
                return false;
            }

            if (_countdownSubscription != null) _countdownSubscription.Dispose(); // 取消订阅 / Dispose the subscription
            StartCountdownInternal(_initTime);
            Dev.Log("Countdown restarted!");
            return true;
        }

        /// <summary>
        /// 停止并销毁倒计时。<br/>
        /// Stops and disposes of the countdown.
        /// </summary>
        public void Stop()
        {
            if (_countdownSubscription == null)
            {
                Dev.Error("Cannot Stop the countdown. It is already finished.");
                return;
            }

            DisposeResources();
        }
    }

}