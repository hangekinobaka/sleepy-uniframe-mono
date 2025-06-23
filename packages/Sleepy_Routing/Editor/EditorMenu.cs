#if UNITY_EDITOR
using UnityEditor;

namespace Sleepy.Routing
{
    public class EditorMenu
    {
        [MenuItem("Sleepy/Routing/Open Route Config", false, 2)]
        public static void ShowRouteWindow()
        {
            RouteConfigWindow.ShowWindow();
        }
    }
}
#endif