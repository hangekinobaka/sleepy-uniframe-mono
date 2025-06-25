using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sleepy.UI
{
    /// <summary>
    /// 你应当把这个类附加到 Slide 物体上。<br/>
    /// You should attach this class to the slide object.
    /// </summary>
    public class Slide : MonoBehaviour
    {
        #region Props

        [Header("If you don't assign these, it will try to find them during runtime")]
        /// <summary>
        /// 此幻灯片的管理 SlideShow 类 / The manager SlideShow class for this slide
        /// </summary>
        public SlideShow MyManager;

        // 此幻灯片的动作列表 / List of actions for this slide
        protected List<Action> _slideActions = new List<Action>();
        // 当前动作的索引 / Index of the current action
        private int _curActionIndex = 0;

        #endregion

        #region Life Cycle

        /// <summary>
        /// 进入此幻灯片之前的操作 / Operations before entering this slide
        /// </summary>
        protected virtual UniTask BeforeEnter()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 进入此幻灯片的操作 / Operations for entering this slide
        /// </summary>
        protected virtual UniTask Enter()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 执行下一个动作之前的操作 / Operations before performing the next action
        /// </summary>
        protected virtual UniTask BeforeNextAction()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 执行下一个动作的操作 / Operations for performing the next action
        /// </summary>
        protected virtual UniTask NextAction()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 退出此幻灯片之前的操作 / Operations before exiting this slide
        /// </summary>
        protected virtual UniTask BeforeExit()
        {
            return UniTask.CompletedTask;
        }

        #endregion

        #region PUBLIC FUNC

        /// <summary>
        /// 进入此幻灯片 / Enter this slide
        /// </summary>
        internal async UniTask EnterSlide()
        {
            await BeforeEnter();

            _curActionIndex = 0;
            gameObject.SetActive(true);

            await Enter();
        }

        /// <summary>
        /// 进行下一个动作 / Go to the next action
        /// </summary>
        internal async UniTask GoNext()
        {
            if (_curActionIndex >= _slideActions.Count)
            {
                // 此幻灯片已完成，进入下一个 / This slide is finished, go to the next one
                await MyManager.GoNextSlide();
            }
            else
            {
                await BeforeNextAction();

                // 触发下一个动作 / Trigger the next action
                _slideActions[_curActionIndex++]?.Invoke();

                await NextAction();
            }
        }

        /// <summary>
        /// 退出此幻灯片 / Exit this slide
        /// </summary>
        internal async UniTask ExitSlide()
        {
            await BeforeExit();

            gameObject.SetActive(false);
        }

        #endregion
    }

}
