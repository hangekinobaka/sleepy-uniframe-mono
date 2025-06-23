using Sleepy.Routing;

namespace Sleepy.Demo.Routing
{
    internal class ChatController : Singleton<ChatController>
    {
        public async void ToggleChatMenu(bool active)
        {
            if (active)
            {
                await Router.LoadPage("ChatMenu");
            }
            else
            {
                await Router.ExitPage("ChatMenu", false, true);
            }
        }

        public async void ToggleConversationWindow(bool active)
        {
            if (active)
            {
                await Router.LoadPage("ConversationWindow");
            }
            else
            {
                await Router.ExitPage("ConversationWindow", false, true);
            }
        }
    }
}
