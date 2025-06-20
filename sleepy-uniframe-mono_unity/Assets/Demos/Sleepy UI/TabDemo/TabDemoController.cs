using Sleepy.UI;
using UniRx;
using UnityEngine;

namespace Sleepy.Demo.UI
{
    internal class TabDemoController : MonoBehaviour
    {
        [SerializeField] TabContainer _tabContainer;

        private void Awake()
        {
            _tabContainer.CurTabIndex.Subscribe(val =>
            {
                Debug.Log($"Tab {val} selected!");
            })
            .AddTo(this)
            .AddTo(_tabContainer);
        }
    }
}