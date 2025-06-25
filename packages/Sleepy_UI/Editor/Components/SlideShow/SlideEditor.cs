#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Sleepy.UI
{

    /// <summary>
    /// 为Slide对象定制的编辑器脚本。<br/>
    /// Custom editor script for the Slide object.
    /// </summary>
    [ExecuteInEditMode]
    [CustomEditor(typeof(Slide), true)]
    public class SlideEditor : Editor
    {
        // 存储对SlideShow组件的引用 / Stores a reference to the SlideShow component
        private SlideShow _slideShow;

        private double _lastUpdateTime = 0;

        /// <summary>
        /// SlideEditor的构造函数，在实例化时自动调用。<br/>
        /// Constructor for SlideEditor, called automatically upon instantiation.
        /// </summary>
        public SlideEditor()
        {
            // 当Hierarchy视图中的内容变化时触发 / Triggered when the contents of the Hierarchy view change
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private void Awake()
        {
            // 获取目标Slide的父级对象 / Get the parent object of the target Slide
            Transform parent = ((Slide)target).transform.parent;
            // 如果_slideShow未初始化，则尝试从父级对象获取SlideShow组件 / If _slideShow is not initialized, try to get the SlideShow component from the parent object
            if (_slideShow == null) _slideShow = parent.GetComponent<SlideShow>();
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
            // 检查当前选中的GameObject是否有Slide组件 / Check if the currently selected GameObject has a Slide component
            if (selectedGameObject != null && selectedGameObject.GetComponent<Slide>() != null)
            {
                Transform parent = selectedGameObject.transform.parent;

                // 遍历同级GameObject / Iterate through sibling GameObjects
                foreach (Transform sibling in parent)
                {
                    Slide siblingSlide = sibling.GetComponent<Slide>();
                    if (siblingSlide != null)
                    {
                        if (sibling.gameObject != selectedGameObject)
                        {
                            // 隐藏其他带有Slide组件的GameObject / Hide other GameObjects with Slide component
                            sibling.gameObject.SetActive(false);
                        }
                        else
                        {
                            // 当选中的是当前Slide时，调用SlideShow的更新方法 / When the selected one is the current Slide, call SlideShow's update method
                            if (_slideShow != null)
                            {
                                _slideShow.OnSlideChange();
                            }
                        }
                    }
                }
            }

            // 如果目标Slide被删除，则调用SlideShow的更新方法 / If the target Slide is deleted, call SlideShow's update method
            if (target == null && _slideShow != null)
            {
                _slideShow.OnSlideChange();
            }
        }
    }
}

#endif