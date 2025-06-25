using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sleepy.UI
{
    internal class ErrorPopup : MonoBehaviour, IErrorPopup
    {
        [Header("Target")]
        [SerializeField] private GameObject _target; // 目标弹窗对象。The target popup object.

        [Header("UI Component")]
        [SerializeField] private TextMeshProUGUI _titleUI; // 标题UI组件。The title UI component.
        [SerializeField] private TextMeshProUGUI _messageUI; // 消息UI组件。The message UI component.
        [SerializeField] private TextMeshProUGUI _cancelButtonTextUI; // 取消按钮文本UI组件。The cancel button text UI component.
        [SerializeField] private Button _cancelButtonUI; // 取消按钮UI组件。The cancel button UI component.

        [Header("Defaults")]
        [SerializeField] private string _defaultCancelText = "OK"; // 默认取消按钮文本。Default text for the cancel button.

        /// <summary>
        /// 显示错误弹窗，包括标题、信息和取消按钮。<br/>
        /// Shows an error popup, including title, message, and a cancel button.
        /// </summary>
        /// <param name="title">弹窗标题。The title of the popup.</param>
        /// <param name="message">错误信息。The error message.</param>
        /// <param name="btnText">取消按钮的文本。The text for the cancel button.</param>
        /// <param name="cancelAction">点击取消时执行的动作。The action to perform on cancel.</param>
        public void ShowError(string title, string message, string btnText, UnityAction cancelAction)
        {
            // Optional title
            if (title == null) _titleUI.gameObject.SetActive(false);
            else
            {
                _titleUI.gameObject.SetActive(true);
                _titleUI.text = title;
            }

            // This is mandatory
            _messageUI.text = message;

            // Optional button text
            if (btnText == null) _cancelButtonTextUI.text = _defaultCancelText;
            else _cancelButtonTextUI.text = btnText;

            // Optional button action(default close popup)
            _cancelButtonUI.onClick.RemoveAllListeners();
            if (cancelAction == null)
            {
                _cancelButtonUI.onClick.AddListener(() =>
                {
                    _target.SetActive(false);
                });
            }
            else
            {
                _cancelButtonUI.onClick.AddListener(cancelAction);
            }

            Dev.Log("Error popup pong!");

            // Move it on top
            _target.transform.SetAsLastSibling();

            // Show it!
            _target.SetActive(true);
        }
    }
}