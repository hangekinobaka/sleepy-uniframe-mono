using Cysharp.Threading.Tasks;
using Sleepy.UI;
using UnityEngine;

namespace Sleepy.Demo.UI
{
    [RequireComponent(typeof(SlideShow))]
    internal class SlideShowDemoController : MonoBehaviour
    {
        SlideShow _slideShow;

        private void Awake()
        {
            _slideShow = GetComponent<SlideShow>();
            _slideShow.ShowSlideShow();
            _slideShow.OnSlideFinished += () => _slideShow.HideSlideShow().Forget();
        }

        private void OnDestroy()
        {
            _slideShow.OnSlideFinished -= () => _slideShow.HideSlideShow().Forget();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _slideShow.GoNext().Forget();
            }
        }
    }
}
