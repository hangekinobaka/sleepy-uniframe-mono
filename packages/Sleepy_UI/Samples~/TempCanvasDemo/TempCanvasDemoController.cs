using Sleepy.UI;
using UnityEngine;

namespace Sleepy.Demo.UI
{
    internal class TempCanvasDemoController : MonoBehaviour
    {
        [SerializeField] GameObject _testUIPrefab;

        public void Show()
        {
            Transform canvas = UIManager.ShowTempCanvas();
            Instantiate(_testUIPrefab, canvas);
        }
    }
}