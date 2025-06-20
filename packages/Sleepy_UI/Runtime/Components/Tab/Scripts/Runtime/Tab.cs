using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Sleepy.UI
{
    public class Tab : MonoBehaviour
    {
        #region Props

        [Header("[READONLY]")]
        public int TabIndex;

        [Header("If you don't assign these, it will try to find them during runtime")]
        public TabContainer MyContainer;

        public bool IsSelected { get; private set; } = false;
        public UnityEvent<bool> OnTabSelectChangedEvent;

        #endregion

        #region Life Cycle

        /// <summary>
        /// 进入此Tab之前的操作 / Operations before entering this Tab
        /// </summary>
        protected virtual UniTask BeforeEnter()
        {
            return UniTask.CompletedTask;
        }
        internal async UniTask BeforeEnterBase()
        {
            await BeforeEnter();
        }

        /// <summary>
        /// 进入此Tab的操作 / Operations for entering this tab
        /// </summary>
        protected virtual UniTask Enter()
        {
            return UniTask.CompletedTask;
        }
        internal async UniTask EnterBase()
        {
            OnTabSelectChangedEvent?.Invoke(true);
            IsSelected = true;
            await Enter();
        }

        /// <summary>
        /// 退出此Tab之前的操作 / Operations before exiting this tab
        /// </summary>
        protected virtual UniTask BeforeExit()
        {
            return UniTask.CompletedTask;
        }
        internal async UniTask BeforeExitBase()
        {
            OnTabSelectChangedEvent?.Invoke(false);
            IsSelected = false;
            await BeforeExit();
        }

        #endregion

        #region PUBLIC FUNC

        /// <summary>
        /// 进入此Tab / Enter this tab
        /// </summary>
        internal async UniTask EnterTab()
        {
            await BeforeEnterBase();

            gameObject.SetActive(true);

            await EnterBase();
        }

        /// <summary>
        /// 退出此Tab / Exit this tab
        /// </summary>
        internal async UniTask ExitTab()
        {
            await BeforeExitBase();

            gameObject.SetActive(false);
        }

        #endregion
    }
}
