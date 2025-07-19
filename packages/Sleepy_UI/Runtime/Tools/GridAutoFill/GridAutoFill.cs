using UnityEngine;
using UnityEngine.UI;

namespace Sleepy.UI.Tool
{
    /// <summary>
    /// 自动填充网格布局组件。<br/>
    /// Automatically fills the grid layout component.
    /// </summary>
    [RequireComponent(typeof(GridLayoutGroup))]
    public class GridAutoFill : MonoBehaviour
    {
        /// <summary>
        /// 网格布局组件。<br/>
        /// The grid layout component.
        /// </summary>
        public GridLayoutGroup _layoutGroup;

        // 限制数量 / Constraint count
        private int _constraintCount;

        // 网格布局的限制类型 / Constraint type of the grid layout
        private UnityEngine.UI.GridLayoutGroup.Constraint _constraint;

        /// <summary>
        /// 在组件属性被修改时调用。<br/>
        /// Called when the component properties are modified.
        /// </summary>
        private void OnValidate()
        {
            if (_layoutGroup == null)
            {
                _layoutGroup = GetComponent<GridLayoutGroup>();
            }
        }

        /// <summary>
        /// 当组件被激活时调用。<br/>
        /// Called when the component is enabled.
        /// </summary>
        private void OnEnable()
        {
            if (_layoutGroup == null)
            {
                _layoutGroup = GetComponent<GridLayoutGroup>();
            }
            UpdateGridLayout();
        }

        /// <summary>
        /// 更新网格布局的配置。<br/>
        /// Updates the grid layout settings.
        /// </summary>
        public void UpdateGridLayout()
        {
            _constraint = _layoutGroup.constraint;
            _constraintCount = _layoutGroup.constraintCount;

            if (_layoutGroup != null && _constraintCount > 0)
            {
                RectTransform parentRect = _layoutGroup.GetComponent<RectTransform>();

                float spacing;
                switch (_constraint)
                {
                    case GridLayoutGroup.Constraint.FixedColumnCount:
                        float containerWidth = parentRect.rect.width;

                        // 计算cell的宽度，考虑到间隔 / Calculate cell width, considering spacing
                        spacing = _layoutGroup.spacing.x * (_constraintCount - 1);
                        float cellWidth = (containerWidth - spacing) / _constraintCount;

                        // 设置cell的大小 / Set cell size
                        _layoutGroup.cellSize = new Vector2(cellWidth, _layoutGroup.cellSize.y);
                        break;
                    case GridLayoutGroup.Constraint.FixedRowCount:
                        float containerHeight = parentRect.rect.height;

                        // 计算cell的高度，考虑到间隔 / Calculate cell height, considering spacing
                        spacing = _layoutGroup.spacing.y * (_constraintCount - 1);
                        float cellHeight = (containerHeight - spacing) / _constraintCount;

                        // 设置cell的大小 / Set cell size
                        _layoutGroup.cellSize = new Vector2(_layoutGroup.cellSize.x, cellHeight);
                        break;
                    case GridLayoutGroup.Constraint.Flexible:
                    default:
                        break;
                }
            }
        }
    }
}
