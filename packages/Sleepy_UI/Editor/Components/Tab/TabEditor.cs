#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Sleepy.UI
{

    /// <summary>
    /// 为Tab对象定制的编辑器脚本。<br/>
    /// Custom editor script for the Tab object.
    /// </summary>
    [ExecuteInEditMode]
    [CustomEditor(typeof(Tab), true)]
    public class TabEditor : Editor
    {
        // 存储对 TabContainer 组件的引用 / Stores a reference to the TabContainer component
        private TabContainer _tabContainer;
        private double _lastUpdateTime = 0;

        /// <summary>
        /// TabEditor的构造函数，在实例化时自动调用。<br/>
        /// Constructor for TabEditor, called automatically upon instantiation.
        /// </summary>
        public TabEditor()
        {
            // 当Hierarchy视图中的内容变化时触发 / Triggered when the contents of the Hierarchy view change
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private void Awake()
        {
            // 检查是否处于游戏预览模式，如果是，则不执行以下代码
            if (EditorApplication.isPlaying)
            {
                return;
            }

            Tab thisTab = (Tab)target;
            // 如果 _tabContainer 未初始化，则尝试从父级对象获取 TabContainer 组件 / If _tabContainer is not initialized, try to get the TabContainer component from the parent object
            if (_tabContainer == null) _tabContainer = thisTab.transform.GetComponentInParent<TabContainer>();
            if (_tabContainer == null)
            {
                Dev.Warning("There is no TabContainer, you tell me why.");
                return;
            }

            int index = 0;
            // 遍历同级GameObject / Iterate through sibling GameObjects
            foreach (Transform sibling in thisTab.transform.parent)
            {
                if (sibling.TryGetComponent<Tab>(out Tab tab))
                {
                    tab.TabIndex = index++;
                }
            }
        }

        /// <summary>
        /// 当Hierarchy视图中的内容变化时调用。<br/>
        /// Called when the contents of the Hierarchy view change.
        /// </summary>
        private void OnHierarchyChanged()
        {
            // 检查是否处于游戏预览模式，如果是，则不执行以下代码
            // 同时限制事件处理频率
            if (EditorApplication.isPlaying || EditorApplication.timeSinceStartup - _lastUpdateTime < 1000)
            {
                return;
            }
            _lastUpdateTime = EditorApplication.timeSinceStartup;

            GameObject selectedGameObject = Selection.activeGameObject;
            // 检查当前选中的GameObject是否有Tab组件 / Check if the currently selected GameObject has a Tab component
            if (selectedGameObject != null && selectedGameObject.GetComponent<Tab>() != null)
            {
                Transform parent = selectedGameObject.transform.parent;

                // 遍历同级GameObject / Iterate through sibling GameObjects
                foreach (Transform sibling in parent)
                {
                    Tab siblingTab = sibling.GetComponent<Tab>();
                    if (siblingTab != null)
                    {
                        if (sibling.gameObject != selectedGameObject)
                        {
                            // 隐藏其他带有Tab组件的GameObject / Hide other GameObjects with Tab component
                            sibling.gameObject.SetActive(false);
                        }
                        else
                        {
                            // 当选中的是当前Tab时，调用 TabContainer 的更新方法 / When the selected one is the current Tab, call TabContainer's update method
                            if (_tabContainer != null)
                            {
                                _tabContainer.OnTabChange();
                            }
                        }
                    }
                }
            }

            // 如果目标Tab被删除，则调用TabShow的更新方法 / If the target Tab is deleted, call TabShow's update method
            if (target == null && _tabContainer != null)
            {
                _tabContainer.OnTabChange();
            }
        }
    }
}

#endif