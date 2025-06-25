using UnityEngine;
using UnityEngine.UI;

namespace Sleepy.Uniframe.Demo
{
    public class TempCanvasDemoUIController : MonoBehaviour
    {
        [SerializeField] Button _closeButton;

        private void OnEnable()
        {
            _closeButton.onClick.AddListener(() => Destroy(gameObject));
        }
    }
}
