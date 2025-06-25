using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sleepy.UI
{
    /// <summary>
    /// 用于控制加载屏幕的控制器。<br/>
    /// Control the loading screen.
    /// </summary>
    public class LoadingScreen : MonoBehaviour, ILoadingScreen
    {
        [Header("Target")]
        [SerializeField] GameObject _target; // 目标对象 / Target object

        [Header("Components")]
        [SerializeField] Image _loadingIcon; // 加载图标 / Loading icon
        [SerializeField] TextMeshProUGUI _loadingText; // 加载文字 / Loading text
        [SerializeField] CanvasGroupFader _fader; // 用于淡入淡出的组件 / Component for fade in/out

        [Header("Setting")]
        [SerializeField] bool _showLoadingIcon = true;
        [SerializeField] bool _showLoadingProgressText = true;

        /// <summary>
        /// If the loading screen is showing
        /// </summary>
        public bool IsShowing => _target.activeSelf;

        private void Awake()
        {
            if (_loadingIcon != null) _loadingIcon.gameObject.SetActive(false);
            if (_loadingText != null) _loadingText.gameObject.SetActive(false);
        }

        private void RenderText(float progress)
        {
            // TODO: Add Localization here 
            _loadingText.text = $"{progress.ToString("P0")}"; // P1 保留一位小数， P2 保留两位小数
        }

        #region ILoadingScreen

        /// <summary>
        /// Shows the loading screen.
        /// </summary>
        /// <param name="loadingScreenStruct">加载屏幕的配置结构体。/ Configuration structure for the loading screen.</param>
        public async void ShowLoadingScreen()
        {
            // If it is alreay shown, do nothing
            if (IsShowing) return;

            // Hide the contents first no matter what
            _loadingIcon?.gameObject.SetActive(false);
            _loadingText?.gameObject.SetActive(false);

            // Show loading screen
            if (_fader != null)
            {
                await _fader.FadeIn();
            }
            else
            {
                _target.SetActive(true);
            }

            // Show corresponding component based on the setting
            if (_showLoadingIcon) _loadingIcon?.gameObject.SetActive(true);
            if (_showLoadingProgressText)
            {
                RenderText(0f);
                _loadingText?.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Hides the loading screen.
        /// </summary>
        public async void HideLoadingScreen()
        {
            // If it is alreay hidden, do nothing
            if (!IsShowing) return;

            // Hide the contents first no matter what
            _loadingIcon?.gameObject.SetActive(false);
            _loadingText?.gameObject.SetActive(false);

            if (_fader != null)
            {
                await _fader.FadeOut();
            }
            else
            {
                _target.SetActive(false);
            }
        }

        /// <summary>
        /// Update the progress UI 
        /// </summary>
        /// <param name="progress"></param>
        public void UpdateLoadingScreen(float progress)
        {
            RenderText(progress);
        }

        #endregion
    }

}