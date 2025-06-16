using System;
using Cysharp.Threading.Tasks;
using Sleepy.Async;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sleepy.UI
{
    /// <summary>
    /// 管理标签页面容器的 MonoBehaviour 类。<br/>
    /// MonoBehaviour class that manages a tab container.
    /// </summary>
    public class TabContainer : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] Transform _content; // 内容区域的引用 / Reference to the content area
        [SerializeField] Transform _headerContent; // 标签头内容区域的引用 / Reference to the header content area
        public Transform HeaderContent
        {
            get
            {
                if (_headerContent == null) _headerContent = transform.FindDeepChild("TabHeaderContent");
                return _headerContent;
            }
        }

        [SerializeField] Transform _header; // 标签头区域的引用 / Reference to the header area
        public Transform Header
        {
            get
            {
                if (_header == null) _header = transform.FindDeepChild("TabHeader");
                return _header;
            }
        }

        [Header("Tab Configs")]
        [SerializeField] int _defaultTabIndex = 0; // 默认标签索引 / Default tab index
        [SerializeField] bool _autoCalcHeaderSize = true; // 是否自动计算标签头大小 / Whether to automatically calculate header size
        [ConditionalDisplay("_autoCalcHeaderSize", true)]
        [Range(1f, 10f)]
        [SerializeField] int _headerCellCount = 3; // 计算标签头单元格数量 / Number of header cells for auto-calculation
        /// <summary>
        /// 获取标签头单元格的数量。<br/>
        /// Gets the number of header cells.
        /// </summary>
        public int HeaderCellCount => _headerCellCount;
        [SerializeField] bool _alwaysStartWithDefault = true;

        [Header("Display Only")]
        [SerializeField] Tab[] _tabs; // 标签数组 / Array of tabs
        [SerializeField] TabButton[] _tabButtons; // 标签按钮数组 / Array of tab buttons

        int _curTabIndex = -1; // 当前标签索引 / Current tab index

        /// <summary>
        /// 当前标签索引的响应属性。<br/>
        /// Reactive property for the current tab index.
        /// </summary>
        public ReactProps<int> CurTabIndex = new ReactProps<int>(0);

        /// <summary>
        /// 获取当前选中的标签按钮。<br/>
        /// Gets the currently selected tab button.
        /// </summary>
        public TabButton CurSelectedButton => _tabButtons[_curTabIndex];

        /// <summary>
        /// 获取当前选中的标签。<br/>
        /// Gets the currently selected tab.
        /// </summary>
        public Tab CurSelectedTab => _tabs[_curTabIndex];

        public UnityEvent OnTabChanged; // 标签更改事件 / Tab changed event

        private void Awake()
        {
            CollectTabs();
            CollectTabButtons();
            AssignTabIndex();
        }

        private async void OnEnable()
        {
            HideTabs();
            await InitTabs();

            await UniTask.DelayFrame(5); // 等待容器大小固定 / Wait for the container size to be fixed
            CalculateHeaderButtonSize();
        }

        #region Event Handlers

        /// <summary>
        /// 处理标签更改事件。<br/>
        /// Handles tab change event.
        /// </summary>
        public void OnTabChange()
        {
            CollectTabs();
        }

        /// <summary>
        /// 处理标签头更改事件。<br/>
        /// Handles tab header change event.
        /// </summary>
        public void OnTabHeaderChange()
        {
            CollectTabButtons();
        }

        #endregion

        #region Local Helpers

        void SetIndex(int index)
        {
            _curTabIndex = index;
            CurTabIndex.SetState(index);
        }

        /// <summary>
        /// 收集并初始化标签。<br/>
        /// Collects and initializes tabs.
        /// </summary>
        void CollectTabs()
        {
            if (_content == null) _content = transform.FindDeepChild("TabContent");
            if (_content == null)
            {
                Dev.Warning("Cannot find a valid tab content object.");
                return;
            }

            // 根据子对象数量初始化标签数组 / Initialize the tabs array based on the number of child objects
            _tabs = new Tab[_content.childCount];

            int index = 0;
            foreach (Transform child in _content)
            {
                if (child.TryGetComponent<Tab>(out Tab tab)) // 确保子对象包含 Tab 脚本 / Ensure the child has a Tab component
                {
                    tab.MyContainer = this;
                    _tabs[index++] = tab;
                }
                else
                {
                    Dev.Warning("You should only put gameobjects with Tab script on it inside the tab content object");
                }
            }
        }

        /// <summary>
        /// 收集并初始化标签按钮。<br/>
        /// Collects and initializes tab buttons.
        /// </summary>
        void CollectTabButtons()
        {
            if (HeaderContent == null)
            {
                Dev.Warning("Cannot find a valid tab header content object.");
                return;
            }

            // 根据子对象数量初始化标签按钮数组 / Initialize the tab buttons array based on the number of child objects
            _tabButtons = new TabButton[HeaderContent.childCount];

            int index = 0;
            foreach (Transform child in HeaderContent)
            {
                if (child.TryGetComponent<TabButton>(out TabButton btn)) // 确保子对象包含 TabButton 脚本 / Ensure the child has a TabButton component
                {
                    btn.MyContainer = this;
                    _tabButtons[index++] = btn;
                }
                else
                {
                    Dev.Warning("You should only put gameobjects with TabButton script on it inside the tab header object");
                }
            }
        }

        /// <summary>
        /// 为标签和标签按钮分配索引。<br/>
        /// Assigns indices to tabs and tab buttons.
        /// </summary>
        void AssignTabIndex()
        {
            for (int i = 0; i < _tabs.Length; i++)
            {
                Tab tab = _tabs[i];
                tab.TabIndex = i;
            }
            for (int i = 0; i < _tabButtons.Length; i++)
            {
                TabButton btn = _tabButtons[i];
                btn.TabIndex = i;
            }
        }

        /// <summary>
        /// 隐藏所有标签。<br/>
        /// Hides all tabs.
        /// </summary>
        void HideTabs()
        {
            foreach (Tab tab in _tabs)
            {
                tab.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 初始化标签。<br/>
        /// Initializes tabs.
        /// </summary>
        async UniTask InitTabs()
        {
            int initIndex;

            if (_alwaysStartWithDefault)
            {
                initIndex = _defaultTabIndex;
            }
            else
            {
                initIndex = _curTabIndex == -1 ? _defaultTabIndex : _curTabIndex;
            }

            if (initIndex < _tabs.Length && initIndex >= 0)
            {
                await ShowTab(initIndex);
            }
            else
            {
                Dev.Warning("The first slide you set is out of the slides range.");
            }
        }

        #endregion

        #region PUBLIC FUNC

        /// <summary>
        /// 显示标签容器。<br/>
        /// Shows the tab container.
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 隐藏标签容器。<br/>
        /// Hides the tab container.
        /// </summary>
        public async UniTask Hide()
        {
            CurSelectedButton.ExitTab();
            await CurSelectedTab.ExitTab();

            HideTabs();

            gameObject.SetActive(false);
        }

        /// <summary>
        /// 显示指定索引的标签。<br/>
        /// Shows the tab at the specified index.
        /// </summary>
        /// <param name="index">要显示的标签的索引。<br/>
        /// The index of the tab to show.</param>
        public async UniTask ShowTab(int index)
        {
            // init case 
            if (_curTabIndex == -1)
            {
                SetIndex(index);
            }
            else if (_curTabIndex != index)
            {
                await BeforeExitTab();

                CurSelectedButton.ExitTab();
                await CurSelectedTab.ExitTab();

                SetIndex(index);
            }

            await BeforeEnterTab();

            CurSelectedButton.EnterTab();
            await CurSelectedTab.EnterTab();

            await EnterTab();
            OnTabChanged?.Invoke();
        }

        /// <summary>
        /// 计算标签头按钮的大小。<br/>
        /// Calculates the size of the header buttons.
        /// </summary>
        public void CalculateHeaderButtonSize()
        {
            if (!_autoCalcHeaderSize) return;

            if (Header == null || HeaderContent == null)
            {
                Dev.Warning("Cannot find the valid tab header objects.");
                return;
            }

            // 如果是水平布局 / If it's a horizontal layout
            HorizontalLayoutGroup layoutGroup = HeaderContent.GetComponent<HorizontalLayoutGroup>();
            if (HeaderContent != null || layoutGroup != null)
            {
                UIUtil.ResizeElementsInHorizontalLayout(layoutGroup, Header, _headerCellCount);
                ScrollRect scrollRect = Header.GetComponentInChildren<ScrollRect>();
                if (scrollRect != null) scrollRect.horizontalNormalizedPosition = 0f;
            }
        }

        #endregion

        #region Life Cycle

        /// <summary>
        /// 进入Tab之前的操作 / Operations before entering Tab
        /// </summary>
        protected virtual UniTask BeforeEnterTab()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 进入Tab的操作 / Operations for entering tab
        /// </summary>
        protected virtual UniTask EnterTab()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 退出Tab之前的操作 / Operations before exiting tab
        /// </summary>
        protected virtual UniTask BeforeExitTab()
        {
            return UniTask.CompletedTask;
        }

        #endregion
    }

}