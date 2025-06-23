using Cysharp.Threading.Tasks;
using Sleepy.Routing;
using UnityEngine;
using UnityEngine.UI;

namespace Sleepy.Demo.Routing
{
    internal class UIController : MonoBehaviour
    {
        const string MENU_PAGE_NAME = "MenuIndex";

        [SerializeField] Button _chatButton;

        private void Awake()
        {
            _chatButton.onClick.AddListener(() => ChatController.Instance.ToggleChatMenu(true));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!Router.IsPageActive(MENU_PAGE_NAME))
                {
                    Router.LoadPage(MENU_PAGE_NAME, true).Forget();
                }
                else
                {
                    Router.ExitPage(MENU_PAGE_NAME).Forget();
                }
            }
        }

    }
}