#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Sleepy.UI
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(TabButton), true)]
    public class TabButtonEditor : Editor
    {// 存储对 TabContainer 组件的引用 / Stores a reference to the TabContainer component
        private TabContainer _tabContainer;

        /// <summary>
        /// TabButtonEditor的构造函数，在实例化时自动调用。<br/>
        /// Constructor for TabButtonEditor, called automatically upon instantiation.
        /// </summary>
        public TabButtonEditor()
        {
            // 当Hierarchy视图中的内容变化时触发 / Triggered when the contents of the Hierarchy view change
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private void Awake()
        {
            TabButton thisTabButton = (TabButton)target;
            // 如果 _tabContainer 未初始化，则尝试从父级对象获取 TabContainer 组件 / If _tabContainer is not initialized, try to get the TabContainer component from the parent object
            if (_tabContainer == null) _tabContainer = thisTabButton.transform.GetComponentInParent<TabContainer>();
            if (_tabContainer == null)
            {
                Dev.Warning("There is no TabContainer, you tell me why.");
                return;
            }

            int index = 0;
            // 遍历同级GameObject / Iterate through sibling GameObjects
            foreach (Transform sibling in thisTabButton.transform.parent)
            {
                if (sibling.TryGetComponent<TabButton>(out TabButton btn))
                {
                    btn.TabIndex = index++;
                }
            }

            if (!Application.isPlaying)
            {
                // 调用 TabContainer 的更新方法 / Call TabContainer's update method
                _tabContainer.OnTabHeaderChange();
                // Ask for size update
                _tabContainer.CalculateHeaderButtonSize();
            }
        }

        /// <summary>
        /// 当Hierarchy视图中的内容变化时调用。<br/>
        /// Called when the contents of the Hierarchy view change.
        /// </summary>
        private void OnHierarchyChanged()
        {
            GameObject selectedGameObject = Selection.activeGameObject;
            // 检查当前选中的GameObject是否有TabButton组件 / Check if the currently selected GameObject has a TabButton component
            if (selectedGameObject != null && selectedGameObject.GetComponent<TabButton>() != null)
            {
                // 调用 TabContainer 的更新方法 / Call TabContainer's update method
                if (_tabContainer != null)
                {
                    _tabContainer.OnTabHeaderChange();
                }
            }

            // 如果目标Tab被删除，则调用TabShow的更新方法 / If the target Tab is deleted, call TabShow's update method
            if (target == null && _tabContainer != null)
            {
                _tabContainer.OnTabHeaderChange();
            }
        }
    }
}

#endif