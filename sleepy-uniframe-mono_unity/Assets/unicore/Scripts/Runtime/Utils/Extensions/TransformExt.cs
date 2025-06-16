using UnityEngine;

namespace Sleepy
{
    public static class TransformExt
    {
        /// <summary>
        /// 在 Transform 的父级中根据名称递归查找 Transform。<br/>
        /// Finds a Transform by name in the parents of the Transform recursively.
        /// </summary>
        /// <param name="me">要开始搜索的 Transform 实例。<br/>
        /// The Transform instance to start the search from.</param>
        /// <param name="name">要查找的 Transform 的名称。<br/>
        /// The name of the Transform to find.</param>
        /// <returns>找到的父级 Transform，如果未找到则为 null。 / The found parent Transform, or null if not found.</returns>
        public static Transform FindInParents(this Transform me, string name)
        {
            // 如果当前节点就是我们要找的 / If the current node is the one we're looking for
            if (me.name == name)
            {
                return me;
            }

            // 如果已经到达了最顶层但还没找到 / If we've reached the topmost level but haven't found it
            if (me.parent == null)
            {
                return null;
            }

            // 否则，继续在父级中寻找 / Otherwise, continue searching in the parent
            return FindInParents(me.parent, name);
        }

        /// <summary>
        /// 在 Transform 的父级中递归查找给定类型的组件。<br/>
        /// Finds a component of a given type in the parents of the Transform recursively.
        /// </summary>
        /// <typeparam name="T">要查找的组件的类型。<br/>
        /// The type of the component to find.</typeparam>
        /// <param name="me">要开始搜索的 Transform 实例。<br/>
        /// The Transform instance to start the search from.</param>
        /// <returns>找到的组件实例，如果未找到则为 null。 / The found component instance, or null if not found.</returns>
        public static T GetComponentInParents<T>(this Transform me) where T : Component
        {
            // 检查当前节点是否拥有我们要找的组件 / Check if the current node has the component we're looking for
            if (me.TryGetComponent<T>(out T t))
            {
                return t;
            }

            // 如果已经到达了最顶层但还没找到 / If we've reached the topmost level but haven't found it
            if (me.parent == null)
            {
                return null;
            }

            // 否则，继续在父级中寻找 / Otherwise, continue searching in the parent
            return GetComponentInParents<T>(me.parent);
        }

        /// <summary>
        /// 在 Transform 的所有子级中深度查找具有指定名称的 Transform。<br/>
        /// Deeply searches for a Transform with a specified name among all children of a Transform.
        /// </summary>
        /// <param name="parent">要搜索的父级 Transform。<br/>
        /// The parent Transform to search in.</param>
        /// <param name="name">要查找的子级 Transform 的名称。<br/>
        /// The name of the child Transform to find.</param>
        /// <returns>找到的 Transform，如果未找到则为 null。 / The found Transform, or null if not found.</returns>
        public static Transform FindDeepChild(this Transform parent, string name)
        {
            // 检查当前层级的子对象 / Check the current level's children
            Transform result = parent.Find(name);
            if (result != null)
            {
                return result;
            }

            // 如果当前层级没有找到，就深入到每一个子对象中继续寻找 / If not found at the current level, delve into each child to continue the search
            foreach (Transform child in parent)
            {
                result = child.FindDeepChild(name);
                if (result != null)
                {
                    return result;
                }
            }

            // 如果所有子对象中都没有找到，返回 null / If not found in any child, return null
            return null;
        }

        /// <summary>
        /// 清除 Transform 的所有子对象。<br/>
        /// Clears all children from a Transform.
        /// </summary>
        /// <param name="transform">要清除子对象的 Transform。/ The Transform from which to clear all children.</param>
        public static void ClearAllChildren(this Transform transform)
        {
            // 逆序循环遍历所有子对象 / Reverse loop through all children
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                // 编辑模式下使用DestroyImmediate而不是Destroy，确保在编辑模式下也能立即销毁对象
                // Use DestroyImmediate instead of Destroy in edit mode to ensure immediate destruction of objects in edit mode
#if UNITY_EDITOR
                if (!Application.isPlaying) Object.DestroyImmediate(transform.GetChild(i).gameObject);
                else Object.Destroy(transform.GetChild(i).gameObject);
#else
        Object.Destroy(transform.GetChild(i).gameObject);
#endif
            }
        }

    }

}
