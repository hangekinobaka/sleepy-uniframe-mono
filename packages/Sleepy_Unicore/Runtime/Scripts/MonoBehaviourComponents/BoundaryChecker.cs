using UnityEngine;

namespace Sleepy.Tool
{
    /// <summary>
    /// 检测GameObject是否超出屏幕边界。如果是则销毁。<br/>
    /// Detecting if the GameObjects is going out of screen boundaries. If yes, destroy it.
    /// </summary>
    public class BoundaryChecker : MonoBehaviour
    {
        /// <summary>
        /// 每帧更新GameObject的位置并检查其是否超出屏幕边界。<br/>
        /// Updates the GameObject's position every frame and checks if it's out of screen boundaries.
        /// </summary>
        void Update()
        {
            // 将GameObject的位置从世界坐标转换为屏幕坐标 / Convert the GameObject's position from world to screen coordinates
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

            // 检查GameObject是否超出屏幕边界 / Check if the GameObject is out of the screen boundaries
            if (screenPosition.x < 0 || screenPosition.x > Screen.width ||
                screenPosition.y < 0 || screenPosition.y > Screen.height)
            {
                Dev.Log($"{this.name} is out of boundary, Destroyed!");
                Destroy(gameObject); // 销毁自己 / Destroy itself
            }
        }
    }

}