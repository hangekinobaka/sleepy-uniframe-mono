#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Sleepy
{
    /// <summary>
    /// Unity 打开时自动检查是否设置了环境宏定义，若未设置则默认设为 DEV_ENV。<br/>
    /// Automatically sets DEV_ENV as the default environment define if none is found when Unity opens.
    /// </summary>
    [InitializeOnLoad]
    public static class EnvironmentAutoInitializer
    {
        // 静态构造函数会在 Unity 编辑器启动或脚本重新编译后立即调用
        // Static constructor runs when Unity editor starts or scripts are recompiled
        static EnvironmentAutoInitializer()
        {
            // 获取当前构建目标组（例如 Standalone、Android 等）<br/>
            // Get the current build target group (e.g., Standalone, Android)
            BuildTargetGroup group = EditorUserBuildSettings.selectedBuildTargetGroup;

            // 获取该构建目标组下的所有宏定义 / Get all scripting define symbols for this group
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);

            // 如果当前没有设置任何环境相关的宏，就默认设为开发环境 / 
            // If no environment define is set, default to DEV_ENV
            if (!defines.Contains(SleepyConsts.DEV_ENV) && !defines.Contains(SleepyConsts.PROD_ENV))
            {
                Debug.LogWarning("<color=#FA8072>[Sleepy EnvironmentAutoInitializer] </color>No environment define found. Setting default to DEV_ENV.");
                EnvironmentSwitcher.SwitchToDevelopment();
            }
        }
    }
}

#endif
