#if UNITY_EDITOR

using UnityEditor;

namespace Sleepy
{
    public class EditorMenu
    {
        [MenuItem("Sleepy/Environment/Switch to Development", false, 0)]
        private static void SwitchEnvDev()
        {
            EnvironmentSwitcher.SwitchToDevelopment();
        }

        [MenuItem("Sleepy/Environment/Switch to Production", false, 1)]
        private static void SwitchEnvProd()
        {
            EnvironmentSwitcher.SwitchToProduction();
        }
    }
}

#endif