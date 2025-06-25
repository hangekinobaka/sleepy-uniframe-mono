using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Sleepy.UI
{
    /// <summary>
    /// 提供幻灯片展示功能的 MonoBehaviour 类。你应当把这个类附加到 Slide 的容器上。<br/>
    /// MonoBehaviour class that provides slideshow functionality. You should attach this class to the slides' container.
    /// </summary>
    public class SlideShow : MonoBehaviour
    {
        [Header("Slide Configs")]
        [SerializeField] int _firstSlideIndex = 0; // 首张幻灯片的索引 / Index of the first slide

        [Header("Display Only")]
        [SerializeField] Slide[] _slides; // 幻灯片列表 / Array of slides

        /// <summary>
        /// 当幻灯片结束时触发的事件。<br/>
        /// Event triggered when the slideshow finishes.
        /// </summary>
        public event UnityAction OnSlideFinished;

        int _curSlideIndex = 0; // 当前幻灯片索引 / Current slide index
        /// <summary>
        /// 当前幻灯片索引<br/> 
        /// Current slide index
        /// </summary>
        public int CurSlideIndex => _curSlideIndex;

        private async void Awake()
        {
            CollectSlides();
            HideSlides();
            await InitSlides();
        }

        #region Event Handlers

        /// <summary>
        /// 当幻灯片更改时调用的事件处理器。<br/>
        /// Event handler called when a slide changes.
        /// </summary>
        public void OnSlideChange()
        {
            CollectSlides();
        }

        #endregion

        #region Local Helpers

        void CollectSlides()
        {
            // 从子元素中收集 Slide 组件 / Collect Slide components from children
            _slides = new Slide[transform.childCount];
            int slideIndex = 0;
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<Slide>(out Slide slide))
                {
                    slide.MyManager = this;
                    _slides[slideIndex++] = slide;
                }
                else
                {
                    Dev.Warning("You should only put gameobjects with Slide script on it inside the SlideShow");
                }
            }
        }

        void HideSlides()
        {
            // 隐藏所有幻灯片 / Hide all slides
            foreach (Slide slide in _slides)
            {
                slide.gameObject.SetActive(false);
            }
        }

        async UniTask InitSlides()
        {
            // 初始化幻灯片 / Initialize slides
            if (_firstSlideIndex < _slides.Length && _firstSlideIndex >= 0)
            {
                await _slides[_firstSlideIndex].EnterSlide();
                _curSlideIndex = _firstSlideIndex;
            }
            else
            {
                Dev.Warning("The first slide you set is out of the slides range.");
            }
        }

        async UniTask GoNextInternal()
        {
            // 内部方法，前往下一张幻灯片 / Internal method to go to the next slide
            await _slides[_curSlideIndex].GoNext();
        }

        async UniTask GoNextSlideInternal()
        {
            // 内部方法，前往下一张幻灯片并处理结束情况 / Internal method to go to the next slide and handle completion
            if (_curSlideIndex + 1 < _slides.Length)
            {
                await _slides[_curSlideIndex++].ExitSlide();
                await _slides[_curSlideIndex].EnterSlide();
            }
            else
            {
                OnSlideFinished?.Invoke();
                Dev.Log("The slide show is finished");
            }
        }

        async UniTask GoPrevSlideInternal()
        {
            // 内部方法，回到上一张幻灯片并处理范围问题 / Internal method to go back to the previous slide and handle range issues
            if (--_curSlideIndex >= 0)
            {
                await _slides[_curSlideIndex].EnterSlide();
            }
            else
            {
                Dev.Warning("The slide you want to enter is out of the slides range.");
            }
        }

        #endregion

        #region PUBLIC FUNC

        /// <summary>
        /// 显示幻灯片展示。<br/>
        /// Shows the slideshow.
        /// </summary>
        public void ShowSlideShow()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 隐藏幻灯片展示。<br/>
        /// Hides the slideshow.
        /// </summary>
        public async UniTask HideSlideShow()
        {
            await _slides[_curSlideIndex].ExitSlide();
            HideSlides();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 前往下一张幻灯片。<br/>
        /// Goes to the next slide.
        /// </summary>
        public async UniTask GoNext()
        {
            await GoNextInternal();
        }

        /// <summary>
        /// 前往下一张幻灯片。<br/>
        /// Goes to the next slide.
        /// </summary>
        public async UniTask GoNextSlide()
        {
            await GoNextSlideInternal();
        }

        /// <summary>
        /// 返回上一张幻灯片。<br/>
        /// Goes back to the previous slide.
        /// </summary>
        public async UniTask GoPrevSlide()
        {
            await GoPrevSlideInternal();
        }

        #endregion
    }

}