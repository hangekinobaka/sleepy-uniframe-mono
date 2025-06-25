using System.Collections;
using UnityEngine;

namespace Sleepy.UI
{
    public class TempCanvasController : MonoBehaviour
    {
        // 存储销毁协程的引用 / Store reference to the destroy coroutine
        private Coroutine _destroyCoroutine;

        void Update()
        {
            // If this GameObject has no children, start the delayed destruction sequence
            if (transform.childCount == 0)
            {
                if (_destroyCoroutine == null)
                {
                    _destroyCoroutine = StartCoroutine(DelayedDestroy(5));
                }
            }
            else
            {
                if (_destroyCoroutine != null)
                {
                    StopCoroutine(_destroyCoroutine);
                    _destroyCoroutine = null;
                }
            }
        }

        /// <summary>
        /// 延迟销毁的协程。<br/>
        /// Coroutine for delayed destruction.
        /// </summary>
        /// <param name="delay">延迟时间，以秒计。 / The delay time in seconds.</param>
        /// <returns>IEnumerator for coroutine.</returns>
        IEnumerator DelayedDestroy(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Destroy the GameObject if it still has no children after the delay
            if (transform.childCount == 0)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 取消并重置销毁过程。<br/>
        /// Cancels and reset the destruction process.
        /// </summary>
        public void CancelDestroy()
        {
            if (_destroyCoroutine != null)
            {
                StopCoroutine(_destroyCoroutine);
                _destroyCoroutine = null;
            }
        }

        void OnDestroy()
        {
            // 当对象销毁时，取消销毁过程
            // When the object is destroyed, cancel the destruction process
            CancelDestroy();
        }
    }
}