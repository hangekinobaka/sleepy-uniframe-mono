using UnityEngine.Events;

namespace Sleepy.UI
{
    internal interface IErrorPopup
    {
        /// <summary>
        /// 显示错误弹窗，包括标题、信息和取消按钮。<br/>
        /// Shows an error popup, including title, message, and a cancel button.
        /// </summary>
        /// <param name="title">弹窗标题。The title of the popup.</param>
        /// <param name="message">错误信息。The error message.</param>
        /// <param name="btnText">取消按钮的文本。The text for the cancel button.</param>
        /// <param name="cancelAction">点击取消时执行的动作。The action to perform on cancel.</param>
        public void ShowError(string title, string message, string btnText, UnityAction cancelAction);
    }
}