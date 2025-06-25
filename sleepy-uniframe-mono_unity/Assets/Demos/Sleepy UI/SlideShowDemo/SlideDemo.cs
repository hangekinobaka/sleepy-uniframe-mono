using Sleepy.UI;
using UnityEngine;

namespace Sleepy.Demo.UI
{
    internal class SlideDemo : Slide
    {
        [SerializeField] Animator _imgAnim;
        [SerializeField] Animator _knightAnim;

        private void Awake()
        {
            // Add slide actions
            _slideActions.Add(ImageEnter);
            _slideActions.Add(ImageGoRight);
        }

        private void ImageEnter()
        {
            _imgAnim.SetTrigger("enter");
        }

        private void ImageGoRight()
        {
            _knightAnim.SetTrigger("run");
            _imgAnim.SetTrigger("leaveRight");
        }
    }
}