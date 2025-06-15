#if UNITY_EDITOR || DEVELOPMENT_BUILD 
using UnityEngine;

namespace Sleepy
{
    /// <summary>
    /// 运行时打印当前环境。<br/>
    /// Logs current environment when the game starts.
    /// </summary>
    public static class EnvironmentLogger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LogCurrentEnvironment()
        {
#if SLEEPY_DEV_ENV
            Debug.Log("<color=#FA8072>[Sleepy Environment]</color> Current environment: <color=#3CB371><b>Development (SLEEPY_DEV_ENV)</b></color>");
#elif SLEEPY_PROD_ENV
            Debug.Log("<color=#FA8072>[Sleepy Environment]</color> Current environment: <color=#1E90FF><b>Production (SLEEPY_PROD_ENV)</b></color>");
#else
            Debug.LogWarning("[Sleepy Environment] No environment macro is defined!");
#endif
        }
    }
}
#endif
