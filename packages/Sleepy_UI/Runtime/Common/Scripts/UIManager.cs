using UnityEngine;

namespace Sleepy.UI
{
    public static class UIManager
    {
        #region Temp Canvas

        private static GameObject _tempCanvas;

        /// <summary>
        /// 显示临时画布，并在必要时创建它。<br/>
        /// Shows the temporary canvas and creates it if necessary.
        /// </summary>
        /// <returns>临时画布的Transform组件。 / The Transform component of the temporary canvas.</returns>
        public static Transform ShowTempCanvas()
        {
            if (_tempCanvas == null)
            {
                GameObject prefab = Resources.Load<GameObject>("TempCanvas");
                _tempCanvas = GameObject.Instantiate(prefab);
            }
            else if (!_tempCanvas.activeSelf)
            {
                _tempCanvas.SetActive(true);
            }
            _tempCanvas.GetComponent<TempCanvasController>().CancelDestroy();

            return _tempCanvas.transform;
        }

        #endregion
    }
}
