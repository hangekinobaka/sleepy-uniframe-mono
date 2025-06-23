#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Sleepy
{
    public static class EditorCommonUtil
    {
        /// <summary>
        /// Load asset with path relative to the Assets/ 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static T LoadAsset<T>(string relativePath) where T : UnityEngine.Object
        {
            return AssetDatabase.LoadAssetAtPath<T>("Assets/" + relativePath);
        }

        /// <summary>
        /// Load Texture2D with path relative to the Assets folder(Editor only) 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Texture2D LoadImage(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        #region predefined icons

        private const string DELETE_ICON_PATH = "Packages/com.sleepy.unicore/Resource/Textures/Icons/delete_icon.png";
        /// <summary>
        /// Load a predefined delete icon(Editor only)
        /// </summary>
        /// <returns></returns>
        public static Texture2D LoadDeleteIcon()
        {
            return LoadImage(DELETE_ICON_PATH);
        }

        #endregion
    }
}

#endif