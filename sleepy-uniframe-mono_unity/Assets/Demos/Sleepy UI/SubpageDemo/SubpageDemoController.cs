using Sleepy.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Sleepy.Demo.UI
{
    internal class SubpageDemoController : MonoBehaviour
    {
        [SerializeField] Button _nextButton;
        [SerializeField] Button _prevButton;

        [SerializeField] SubpageManager _subpageManager;

        private void OnEnable()
        {
            _nextButton.onClick.AddListener(() => _subpageManager.GoNextSubpage(true));
            _prevButton.onClick.AddListener(() => _subpageManager.GoPrevSubpage(true));
        }

        private void OnDisable()
        {
            _nextButton.onClick.RemoveAllListeners();
            _prevButton.onClick.RemoveAllListeners();
        }

    }
}