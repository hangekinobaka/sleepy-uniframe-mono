#if UNITY_EDITOR

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Sleepy
{
    /// <summary>
    /// 管理环境之间切换的静态类。<br/>
    /// Static class to manage switching between environments.
    /// </summary>
    public static class EnvironmentSwitcher
    {
        /// <summary>
        /// 切换到开发环境。<br/>
        /// Switch to the development environment.
        /// </summary>
        public static void SwitchToDevelopment()
        {
            SetDefineSymbols(SleepyConsts.DEV_ENV);
        }

        /// <summary>
        /// 切换到生产环境。<br/>
        /// Switch to the production environment.
        /// </summary>
        public static void SwitchToProduction()
        {
            SetDefineSymbols(SleepyConsts.PROD_ENV);
        }

        /// <summary>
        /// 设置环境特定的定义符号。<br/>
        /// Sets environment-specific define symbols.
        /// </summary>
        /// <param name="define">要设置的定义符号。/ The define symbol to set.</param>
        private static void SetDefineSymbols(string define)
        {
            // 获取当前活跃的构建目标组 / Get the currently active build target group
            BuildTargetGroup activeGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            // 获取该构建目标组当前的定义符号 / Get the current define symbols for this build target group
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(activeGroup);

            // 移除与环境相关的旧定义符号 / Remove existing defines related to environment
            defines = RemoveDefine(defines, SleepyConsts.DEV_ENV);
            defines = RemoveDefine(defines, SleepyConsts.PROD_ENV);

            // 添加新定义，检查是否已存在以避免重复添加 / Add new define, check if it already exists to avoid duplication
            if (!defines.Contains(define))
            {
                defines += (string.IsNullOrEmpty(defines) ? "" : ";") + define;
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(activeGroup, defines);


            Debug.Log($"<color=#FA8072>[Sleepy EnvironmentSwitcher] </color>Switched to {define}.");
        }

        /// <summary>
        /// 从定义符号字符串中移除特定定义符号。<br/>
        /// Removes a specific define symbol from the define symbols string.
        /// </summary>
        /// <param name="defines">包含定义符号的字符串。/ The string containing define symbols.</param>
        /// <param name="define">要移除的定义符号。/ The define symbol to remove.</param>
        /// <returns>更新后的定义符号字符串。/ Updated define symbols string.</returns>
        private static string RemoveDefine(string defines, string define)
        {
            var allDefines = defines.Split(';');
            defines = string.Join(";", allDefines.Where(d => d != define).ToArray());
            return defines;
        }
    }

}

#endif
