using System;
using UnityEngine;

namespace Sleepy.Tool
{
    /// <summary>
    /// 提供防抖功能的类。<br/>
    /// Class that provides debouncing functionality.
    /// </summary>
    public class Debouncer
    {
        // 上一次触发操作的时间 / Last time an action was invoked
        private float _lastInvokeTime = 0f;

        /// <summary>
        /// 使用防抖时间执行指定的操作。<br/>
        /// Executes the specified action with debounce time.
        /// </summary>
        /// <param name="action">要执行的操作。<br/>The action to execute.</param>
        /// <param name="debounceTime">防抖时间（毫秒）。<br/>Debounce time in milliseconds.</param>
        public void RunWithDebounce(Action action, int debounceTime)
        {
            // 获取当前时间 / Get the current time
            float currentTime = Time.time;

            // 检查是否是首次执行或者已过防抖时间 / Check if it's the first execution or the debounce time has passed
            if (_lastInvokeTime == 0 || (currentTime - _lastInvokeTime) * 1000f >= debounceTime)
            {
                action.Invoke(); // 执行操作 / Execute the action

                // 更新最后执行时间 / Update the last invoke time
                _lastInvokeTime = currentTime;
            }
        }
    }
}