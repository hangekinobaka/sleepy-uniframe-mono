using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sleepy.UI
{
    /// <summary>
    /// 表示一个标签按钮的 MonoBehaviour 类。<br/>
    /// MonoBehaviour class representing a tab button.
    /// </summary>
    public class TabButton : MonoBehaviour
    {
        #region Props

        // 按钮组件的引用 （你必须安排一个Button UI）<br/>
        // Reference to the button component
        [Header("Refs")]
        [SerializeField] Button _button;
        /// <summary>
        /// 获取按钮组件。<br/>
        /// Gets the button component.
        /// </summary>
        public Button Button => _button;

        [Header("[READONLY]")]
        public int TabIndex; // 标签的索引 / Index of the tab

        [Header("If you don't assign these, it will try to find them during runtime")]
        /// <summary>
        /// 获取或设置所属的标签容器。<br/>
        /// Gets or sets the parent tab container.
        /// </summary>
        public TabContainer MyContainer;

        /// <summary>
        /// 获取标签是否被选中。<br/>
        /// Gets whether the tab is selected.
        /// </summary>
        public bool IsSelected { get; private set; }

        public UnityEvent<bool> OnTabSelectChangedEvent; // 标签选中状态改变事件 / Tab select state changed event

        #endregion

        private void Awake()
        {
            if (_button == null)
            {
                _button = GetComponentInChildren<Button>();
            }

            if (_button == null)
            {
                Dev.Error("You must give TabButton a Button UI inside it!");
            }
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(async () =>
            {
                await MyContainer.ShowTab(TabIndex);
            });
        }

        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }

        #region PUBLIC FUNC

        /// <summary>
        /// 进入此标签。<br/>
        /// Enters this tab.
        /// </summary>
        internal void EnterTab()
        {
            IsSelected = true;
            if (_button == null)
            {
                _button = GetComponentInChildren<Button>();
            }
            if (_button != null) _button.interactable = false;
            OnTabSelectChangedEvent?.Invoke(true);
        }

        /// <summary>
        /// 退出此标签。<br/>
        /// Exits this tab.
        /// </summary>
        internal void ExitTab()
        {
            IsSelected = false;
            if (_button == null)
            {
                _button = GetComponentInChildren<Button>();
            }
            if (_button != null) _button.interactable = true;
            OnTabSelectChangedEvent?.Invoke(false);
        }

        #endregion
    }

}