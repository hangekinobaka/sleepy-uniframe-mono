using Sleepy.Routing;
using UnityEngine;

namespace Sleepy.Demo.Routing
{
    internal class MainMenuController : MonoBehaviour
    {
        public async void SwitchSetting()
        {
            await Router.SwitchPage("SettingPage");
        }

        public async void SwitchCredit()
        {
            await Router.SwitchPage("CreditPage");
        }

        public async void Close()
        {
            await Router.ExitPage("MenuIndex");
        }

        public async void GoBack()
        {

            await Router.GoBack(true);
        }
    }
}