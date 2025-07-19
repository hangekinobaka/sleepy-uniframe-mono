#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Sleepy.Routing
{
    public class GameObjectMenuHelper : MonoBehaviour
    {
        /// <summary>
        /// Copy the hierarchy path to the clipboard
        /// </summary>
        [MenuItem("GameObject/Sleepy/Copy Path", false, 0)]
        private static void CopyPath()
        {
            if (Selection.activeGameObject == null) return;

            string path = GetHierarchyPath(Selection.activeGameObject);
            EditorGUIUtility.systemCopyBuffer = path;
            Dev.Log(path);

        }

        /// <summary>
        /// Generate the page path and save it in the path sheet
        /// </summary>
        [MenuItem("GameObject/Sleepy/Copy Route", false, 1)]
        public static void CopyRoute()
        {
            if (Selection.activeGameObject == null) return;

            string route = GetHierarchyPathWithSceneName(Selection.activeGameObject);
            EditorGUIUtility.systemCopyBuffer = route;
            Dev.Log(route);
        }


        #region Utildity Methods

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
    }
}

#endif