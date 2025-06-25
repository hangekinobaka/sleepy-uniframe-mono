using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Sleepy.UI
{
    /// <summary>
    /// 用于 CanvasGroup 淡入淡出的组件。<br/>
    /// Component for controlling fade in and fade out of a CanvasGroup.
    /// </summary>
    public class CanvasGroupFader : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] CanvasGroup _canvasGroup; // 目标画布组 / Target CanvasGroup

        [Header("Settings")]
        [SerializeField] float _fadeInDuration = 2.0f; // 淡入持续时间 / Fade-in duration
        [SerializeField] float _fadeOutDuration = 2.0f; // 淡出持续时间 / Fade-out duration

        public event UnityAction OnFadeInFinished; // 淡入完成事件 / Event for fade-in completion
        public event UnityAction OnFadeOutFinished; // 淡出完成事件 / Event for fade-out completion

        /// <summary>
        /// 如果你没有手动设置 CanvasGroup， 初始化时检查并获取当前物件上的 CanvasGroup。<br/>
        /// Checks and gets the CanvasGroup on initialization if you did not manully assign one.
        /// </summary>
        void Awake()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                {
                    Dev.Error("No valid CanvasGroup set in CanvasGroupFader");
                    return;
                }
            }
        }

        /// <summary>
        /// 执行淡入效果。<br/>
        /// Performs the fade-in effect.
        /// </summary>
        public async UniTask FadeIn()
        {
            _canvasGroup.alpha = 0;
            gameObject.SetActive(true);
            await FadeCanvasGroup(0, 1, _fadeInDuration);

            OnFadeInFinished?.Invoke();
        }

        /// <summary>
        /// 执行淡出效果。<br/>
        /// Performs the fade-out effect.
        /// </summary>
        public async UniTask FadeOut()
        {
            await FadeCanvasGroup(1, 0, _fadeOutDuration);
            gameObject.SetActive(false);

            OnFadeOutFinished?.Invoke();
        }

        /// <summary>
        /// 实现渐变效果。<br/>
        /// Implements fade.
        /// </summary>
        /// <param name="start">起始透明度。/ Starting alpha value.</param>
        /// <param name="end">结束透明度。/ Ending alpha value.</param>
        /// <param name="lerpTime">过渡时间，默认为2秒。/ Lerp time, default is 2 seconds.</param>
        public async UniTask FadeCanvasGroup(float start, float end, float lerpTime = 2.0f)
        {
            float startTime = Time.time;
            float timePassed;
            float percentageComplete;

            while (true)
            {
                timePassed = Time.time - startTime;
                percentageComplete = timePassed / lerpTime;

                _canvasGroup.alpha = Mathf.Lerp(start, end, percentageComplete);

                if (percentageComplete >= 1) break;

                await UniTask.Yield();
            }
        }
    }

}
