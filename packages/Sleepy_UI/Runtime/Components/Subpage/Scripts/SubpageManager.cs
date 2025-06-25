using UnityEngine;

namespace Sleepy.UI
{
    /// <summary>
    /// 管理UI子页面的类。<br/>
    /// Class for managing UI subpages.
    /// </summary>
    public class SubpageManager : MonoBehaviour
    {
        [Header("Assign all your subpages here")]
        [SerializeField] GameObject[] _subpages;

        [Header("Configs")]
        [SerializeField] int _defaultStartIndex = 0;

        /// <summary>
        /// 当前激活的子页面索引。<br/>
        /// The index of the currently active subpage.
        /// </summary>
        public int ActiveSubpageIndex { get; private set; } = 0;

        private void Awake()
        {
            ActiveSubpageIndex = _defaultStartIndex;
            ShowSubpageInternal();
        }

        #region Internal

        // 显示特定的子页面
        // Show a specific subpage
        private void ShowSubpageInternal()
        {
            if (ActiveSubpageIndex < 0 || ActiveSubpageIndex >= _subpages.Length)
            {
                // The subpage index is out of range.
                Dev.Warning("The subpage index is out of range.");
                return;
            }

            // 隐藏所有其他页面并显示选定的页面
            // Hide all other pages and show the selected one
            for (int i = 0; i < _subpages.Length; i++)
            {
                _subpages[i].SetActive(ActiveSubpageIndex == i);
            }
        }

        #endregion

        #region PUBLIC

        /// <summary>
        /// 显示与ActiveSubpageIndex相同索引的页面。也可以传入索引号进行显示，并同时修改ActiveSubpageIndex。<br/>
        /// Show the page with the same index as the ActiveSubpageIndex. Or you can pass in an index number and show it, also modify the ActiveSubpageIndex the same time.
        /// </summary>
        /// <param name="index">[可选] 同时更改ActiveSubpageIndex / [Optional] Also change the ActiveSubpageIndex</param>
        public void ShowSubpage(int index = -1)
        {
            if (index != -1)
            {
                ActiveSubpageIndex = index;
            }

            ShowSubpageInternal();
        }

        /// <summary>
        /// 转到下一个子页面。<br/>
        /// Go to the next subpage.
        /// </summary>
        /// <param name="loop">如果为true，到达最后一个页面时循环到第一个页面。/ If true, loop to the first page when reaching the last one.</param>
        public void GoNextSubpage(bool loop = false)
        {
            if (_subpages.Length == 0)
            {
                // You have 0 subpage.
                Dev.Warning("You have 0 subpage");
                return;
            }

            ActiveSubpageIndex = (ActiveSubpageIndex + 1 >= _subpages.Length) ?
                loop ? 0 : _subpages.Length - 1
                : ActiveSubpageIndex + 1;

            ShowSubpageInternal();
        }

        /// <summary>
        /// 转到前一个子页面。<br/>
        /// Go to the previous subpage.
        /// </summary>
        /// <param name="loop">如果为true，从第一个页面循环到最后一个页面。/ If true, loop to the last page when reaching the first one.</param>
        public void GoPrevSubpage(bool loop = false)
        {
            if (_subpages.Length == 0)
            {
                // You have 0 subpage.
                Dev.Warning("You have 0 subpage");
                return;
            }

            ActiveSubpageIndex = (ActiveSubpageIndex - 1 < 0) ?
                loop ? _subpages.Length - 1 : 0
                : ActiveSubpageIndex - 1;

            ShowSubpageInternal();
        }

        #endregion
    }
}
