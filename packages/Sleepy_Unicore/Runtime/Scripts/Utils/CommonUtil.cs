using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Sleepy
{
    public static class CommonUtil
    {
        #region Hierarchy

        /// <summary>
        /// Get hierarchy path
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetHierarchyPath(GameObject obj)
        {
            string path = obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = obj.name + "/" + path;
            }
            return path;
        }

        /// <summary>
        /// Get SceneName + hierarchy path
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetHierarchyPathWithSceneName(GameObject obj)
        {
            // 使用obj.scene获取GameObject所在的场景
            string sceneName = obj.scene.name;
            string hierarchyPath = GetHierarchyPath(obj);

            string path = $"{sceneName}/{hierarchyPath}";
            return path;
        }

        #endregion

        #region File IO

        /// <summary>
        /// 验证文件路径是否有效并统一路径分隔符。<br/>
        /// Validates the file path and unifies the path separators.
        /// </summary>
        /// <param name="path">要验证的文件路径。/ The file path to validate</param>
        /// <returns>处理后的文件路径。/ The processed file path</returns>
        public static string ValidateFilePath(string path)
        {
            // 替换反斜杠为正斜杠，确保路径分隔符一致 / Replace backslashes with forward slashes to ensure consistent path separators
            return Regex.Replace(path, @"[\\]+", "/");
        }

        /// <summary>
        /// 检查并保存文件路径，如果路径不存在则创建它。<br/>
        /// Checks and saves the file path, creating it if it does not exist.
        /// </summary>
        /// <param name="path">要检查和保存的文件路径。/ The file path to check and save</param>
        /// <param name="doLog">是否记录日志。/ Whether to log the operation</param>
        /// <returns>路径检查和保存是否成功。/ Whether the path check and save operation was successful</returns>
        public static bool SaveFilePathCheck(ref string path, bool doLog = true)
        {
            path = ValidateFilePath(path);
            try
            {
                if (!File.Exists(path))
                {
                    // 创建文件目录 / Create the file directory
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    if (doLog) Dev.Log($"create {path}");
                }
                return true;
            }
            catch (System.Exception)
            {
                Dev.Error($"{path} does not exist and cannot be created!");
                return false;
            }
        }

        /// <summary>
        /// 以T的格式读取JSON<br/>
        /// Load JSON file and get the data in T format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="result"></param>
        /// <returns>If load is succeed</returns>
        public static bool TryLoadJSON<T>(string path, out T result, bool doLog = true)
        {
            result = default(T);

            path = ValidateFilePath(path);
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            if (textAsset != null)
            {
                string json = textAsset.text;
                result = JsonUtility.FromJson<T>(json);
                if (doLog) Dev.Log($"Load data from {path}");
                return true;
            }

            if (doLog) Dev.Error($"Cannot get file from path `{path}`");
            return false;
        }

        /// <summary>
        /// 把 T格式 的数据存储为 JSON 文件<br/>
        /// Store T format data as a JSON file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="doLog"></param>
        public static void SaveJSON<T>(string path, T data, bool doLog = true)
        {
            SaveFilePathCheck(ref path, doLog);

            // 保存JSON文件
            // Save the JSON file
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);

            // 记录日志
            // Log the action
            if (doLog) Dev.Log($"Saved to file {path}");

            // 刷新Unity编辑器的Project视图
            // Refresh Unity Editor's Project view
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        #endregion

        #region Convert

        /// <summary>
        /// 将秒转换为毫秒。 / Converts seconds to milliseconds.
        /// </summary>
        /// <param name="seconds">要转换的秒数。比如 Time.time / The seconds to convert.</param>
        /// <returns>转换后的毫秒数。 / The converted milliseconds.</returns>
        public static int ToMilliseconds(float seconds)
        {
            return (int)(seconds * 1000f);
        }

        #endregion
    }
}