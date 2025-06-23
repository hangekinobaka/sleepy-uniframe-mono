using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sleepy.Routing
{
    /// <summary>
    /// 为页面提供基础功能和生命周期事件的类。<br/>
    /// Provide basic functionality and lifecycle events for a page.
    /// </summary>
    public class Page : MonoBehaviour
    {
        #region VIRTUAL

        // 页面初始化时调用（第一次）
        // Called when the page is initialized (The first time)
        protected virtual UniTask OnInit() { return UniTask.CompletedTask; }

        // 由路由器激活页面时调用（每次）
        // Called when the page is activated by the router (Every time)
        protected virtual UniTask OnEnter(object dataTransferObject = null) { return UniTask.CompletedTask; }

        // 页面暂停时调用
        // Called when the page is paused
        protected virtual UniTask OnPause() { return UniTask.CompletedTask; }

        // 从暂停中恢复页面时调用
        // Called when the page is resumed from pause
        protected virtual UniTask OnResume() { return UniTask.CompletedTask; }

        #endregion

        #region PROPS
        /// <summary>
        /// 本页面的路由信息。<br/>
        /// The route info of this page.
        /// </summary>
        [Tooltip("Page name will be filled when this page is inited by the router. You don't need to write it down here.")]
        [SerializeField] protected string _pageName;

        /// <summary>
        /// 指示页面是否已初始化。<br/>
        /// Indicates whether the page has been initialized.
        /// </summary>
        public bool IsInited { get; private set; } = false;

        /// <summary>
        /// 此页面是否为激活状态。<br/>
        /// Indicates whether the page is in active.
        /// </summary>
        public bool IsActive { get; private set; } = false;

        #endregion

        #region PUBLIC BASE FUNC

        /// <summary>
        /// 基础初始化函数，设置路由信息并标记为已初始化。<br/>
        /// Base initialization function, sets route info and marks as initialized.
        /// </summary>
        /// <param name="routePage">页面的路由信息。<br/>The route info for the page.</param>
        internal async UniTask OnInitBase(string name)
        {
            // 通用初始化操作 / Common Init actions
            _pageName = name;
            IsInited = true;

            await OnInit();
        }

        /// <summary>
        /// 基础进入函数，调用 OnEnter 生命周期事件。<br/>
        /// Base enter function, calls the OnEnter lifecycle event.
        /// </summary>
        internal async UniTask OnEnterBase(object dataTransferObject = null)
        {
            IsActive = true;

            await OnEnter(dataTransferObject);
        }

        /// <summary>
        /// 基础暂停函数，调用 OnPause 生命周期事件。<br/>
        /// Base pause function, calls the OnPause lifecycle event.
        /// </summary>
        internal virtual async UniTask OnExitBase()
        {
            IsActive = false;
            await OnPause();
        }

        /// <summary>
        /// 基础恢复函数，调用 OnResume 生命周期事件。<br/>
        /// Base resume function, calls the OnResume lifecycle event.
        /// </summary>
        internal virtual async UniTask OnResumeBase()
        {
            await OnResume();
        }

        #endregion

        /// <summary>
        /// You should not use Awake in your Page class
        /// </summary>
        private void Awake()
        {
            if (!IsActive) gameObject.SetActive(false);
        }
    }

}