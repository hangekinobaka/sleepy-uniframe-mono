#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Sleepy.UI
{

    /// <summary>
    /// 为TabContainer对象定制的编辑器脚本。<br/>
    /// Custom editor script for the TabContainer object.
    /// </summary>
    [ExecuteInEditMode]
    [CustomEditor(typeof(TabContainer), true)]
    public class TabContainerEditor : Editor
    {
        TabContainer _tabContainer;

        int _headerCellCountCache = 0;
        float _headerWidthCache = 0;
        RectTransform _headerRect;

        /// <summary>
        /// TabContainerEditor的构造函数，在实例化时自动调用。<br/>
        /// Constructor for TabContainerEditor, called automatically upon instantiation.
        /// </summary>
        public TabContainerEditor()
        {
        }

        private void Awake()
        {
            _tabContainer = (TabContainer)target;
            _headerCellCountCache = _tabContainer.HeaderCellCount;
            _headerRect = _tabContainer.Header.GetComponent<RectTransform>();
            _headerWidthCache = _headerRect.rect.width;
        }

        public override void OnInspectorGUI()
        {
            // 这里确保了每次Inspector绘制时，_tabContainer都是最新的
            _tabContainer = (TabContainer)target;

            // 在这里调用base.OnInspectorGUI()，将会绘制默认的Inspector界面
            base.OnInspectorGUI();

            // 监听_tabContainer的特定字段变化
            if (_headerCellCountCache != _tabContainer.HeaderCellCount || _headerWidthCache != _headerRect.rect.width)
            {
                _headerCellCountCache = _tabContainer.HeaderCellCount;
                _headerWidthCache = _headerRect.rect.width;
                _tabContainer.CalculateHeaderButtonSize();
            }
        }
    }
}

#endif