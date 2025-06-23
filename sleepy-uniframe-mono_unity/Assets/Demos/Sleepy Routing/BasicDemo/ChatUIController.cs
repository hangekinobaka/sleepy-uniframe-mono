using UnityEngine;
using UnityEngine.UI;

namespace Sleepy.Demo.Routing
{
    internal class ChatUIController : MonoBehaviour
    {
        [SerializeField] Button _closeMenuButton;
        [SerializeField] Button _closeWindowButton;

        private void Awake()
        {
            _closeMenuButton.onClick.AddListener(() => ChatController.Instance.ToggleChatMenu(false));
            _closeWindowButton.onClick.AddListener(() => ChatController.Instance.ToggleConversationWindow(false));
        }

        private void OnDestroy()
        {
            _closeMenuButton.onClick.RemoveAllListeners();
            _closeWindowButton.onClick.RemoveAllListeners();
        }

        public void OpenConversationWindow()
        {
            ChatController.Instance.ToggleConversationWindow(true);
        }

    }
}
