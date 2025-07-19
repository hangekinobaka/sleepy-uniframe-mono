using Sleepy.Tool;
using UnityEngine;
using UnityEngine.UI;

namespace Sleepy.Demo
{
    internal class DebounceTest : MonoBehaviour
    {
        #region UI Toolbox Test
        public void OnClickedHandler(string text)
        {
            Dev.Log($"{text} clicked!");
        }

        public void OnToggleHandler(string text)
        {
            Dev.Log($"{text} toggle");
        }

        #endregion

        #region Debouncer Test

        Debouncer _buttonDebouncer = new Debouncer();
        [SerializeField] Button _button2;

        Debouncer _animDebouncer = new Debouncer();
        [SerializeField] Animator _anim;
        private void OnEnable()
        {
            _button2.onClick.AddListener(() => _buttonDebouncer.RunWithDebounce(
                () => Dev.Log("Debouncer: Button 2 is clicked!"), 500
            ));
        }

        private void OnDisable()
        {
            _button2.onClick.RemoveAllListeners();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _animDebouncer.RunWithDebounce(
                    () => _anim.SetTrigger("jump"),
                    1000
                    );
            }


        }

        public void SomeAction(int i)
        {
            Debug.Log(i);
        }

        #endregion
    }
}