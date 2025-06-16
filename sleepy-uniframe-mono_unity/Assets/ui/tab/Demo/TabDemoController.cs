using UniRx;
using UnityEngine;

namespace Sleepy.UI.Demo
{
    internal class TabDemoController : MonoBehaviour
    {
        [SerializeField] TabContainer _tabContainer;

        private void Awake()
        {
            _tabContainer.CurTabIndex.State.Subscribe(val =>
            {
                Debug.Log($"Tab {val} selected!");
            })
            .AddTo(this)
            .AddTo(_tabContainer);
        }
    }
}