#if UNITY_EDITOR

using UnityEditor;
using UnityEngine.UI;

namespace Sleepy.UI.Tool
{
    /// <summary>
    /// GridAutoFill 的自定义编辑器。<br/>
    /// Custom editor for GridAutoFill.
    /// </summary>
    [CustomEditor(typeof(GridAutoFill))]
    public class GridAutoFillEditor : Editor
    {
        /// <summary>
        /// GridAutoFill 脚本实例。<br/>
        /// GridAutoFill script instance.
        /// </summary>
        private GridAutoFill _gridAutoFillScript;

        // 网格布局组件 / Grid layout component
        private GridLayoutGroup _gridLayoutGroup;

        // Caches
        private int _previousConstraintCount;
        private UnityEngine.UI.GridLayoutGroup.Constraint _previousConstraint;

        void OnEnable()
        {
            _gridAutoFillScript = (GridAutoFill)target;
            _gridLayoutGroup = _gridAutoFillScript.GetComponent<GridLayoutGroup>();
            _previousConstraint = _gridLayoutGroup.constraint;
            _previousConstraintCount = _gridLayoutGroup.constraintCount;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_gridLayoutGroup.constraintCount != _previousConstraintCount
                || _previousConstraint != _gridLayoutGroup.constraint)
            {
                // constraintCount 发生变化时，执行所需的操作
                _previousConstraint = _gridLayoutGroup.constraint;
                _previousConstraintCount = _gridLayoutGroup.constraintCount;
                _gridAutoFillScript.UpdateGridLayout();
            }
        }
    }
}

#endif
