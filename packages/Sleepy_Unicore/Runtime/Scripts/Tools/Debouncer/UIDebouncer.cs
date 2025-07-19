using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Sleepy.Tool
{
    /// <summary>
    /// 提供UI元素防抖功能的组件。<br/>
    /// A component that provides debouncing functionality for UI elements.
    /// </summary>
    public class UIDebouncer : MonoBehaviour, IPointerClickHandler // 实现IPointerClickHandler接口来监听点击事件 / Implements IPointerClickHandler interface to listen for click events
    {
        [SerializeField] UnityEvent _onClickDebounced; // 可以在Inspector中绑定的事件 / Event that can be bound in the Inspector
        [SerializeField] int _debounceTime = 500; // 防抖时间，以毫秒为单位 / Debounce time in milliseconds

        private bool _isDebouncing = false; // 标记是否正在防抖中 / Flag to indicate if debouncing is in progress

        /// <summary>
        /// 点击事件处理，实现防抖逻辑。<br/>
        /// Handles click events and implements debounce logic.
        /// </summary>
        /// <param name="eventData">点击事件的数据。/ The data of the click event.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isDebouncing)
            {
                _isDebouncing = true; // 开始防抖 / Start debouncing
                _onClickDebounced?.Invoke(); // 触发绑定的事件 / Trigger the bound event
                StartCoroutine(DebounceCoroutine()); // 启动协程进行延时 / Start coroutine for delay
            }
        }

        /// <summary>
        /// 协程实现防抖延时。<br/>
        /// Coroutine for debounce delay.
        /// </summary>
        private System.Collections.IEnumerator DebounceCoroutine()
        {
            yield return new WaitForSeconds(_debounceTime / 1000f); // 等待指定毫秒数 / Wait for specified milliseconds
            _isDebouncing = false; // 防抖结束 / End debouncing
        }
    }
}