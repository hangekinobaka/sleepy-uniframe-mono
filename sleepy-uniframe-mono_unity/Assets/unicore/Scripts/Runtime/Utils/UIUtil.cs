using UnityEngine;
using UnityEngine.UI;

namespace Sleepy
{
    public static class UIUtil
    {
        /// <summary>
        /// 根据参考 Transform 的宽度和指定的单元格数量，调整水平布局组中元素的大小。<br/>
        /// Resizes elements in a horizontal layout group based on the width of a reference Transform and a specified number of cells.
        /// </summary>
        /// <param name="layoutGroup">要调整大小的元素所在的水平布局组。<br/>
        /// The horizontal layout group containing the elements to resize.</param>
        /// <param name="referenceTransform">作为宽度参考的 Transform。<br/>
        /// The Transform to use as a reference for width.</param>
        /// <param name="cellCount">水平布局中的单元格数量。<br/>
        /// The number of cells in the horizontal layout.</param>
        public static void ResizeElementsInHorizontalLayout(HorizontalLayoutGroup layoutGroup, Transform referenceTransform, int cellCount)
        {
            // 获取参考Transform的RectTransform组件以获取其宽度 / Get the RectTransform component of the reference Transform to obtain its width
            RectTransform refRectTransform = referenceTransform.GetComponent<RectTransform>();
            float referenceWidth = refRectTransform.rect.width;

            // 计算每个元素应该有的宽度 / Calculate the width each element should have
            float elementWidth = (referenceWidth - (layoutGroup.spacing * (cellCount - 1))) / cellCount;

            // 遍历布局中的所有子元素 / Iterate through all children in the layout
            foreach (Transform child in layoutGroup.transform)
            {
                // 获取子元素的Layout Element组件，如果没有就添加一个 / Get the Layout Element component of the child, adding one if it's missing
                LayoutElement le = child.GetComponent<LayoutElement>();
                if (le == null)
                {
                    le = child.gameObject.AddComponent<LayoutElement>();
                }

                // 设置子元素的宽度 / Set the width of the child element
                le.minWidth = elementWidth;
                le.preferredWidth = elementWidth;
            }

            Canvas.ForceUpdateCanvases();
        }

        #region UI Screen Position

        /// <summary>
        /// 计算UI元素完全移出屏幕时的位置<br/>
        /// Calculate the position of a UI element when it's completely off-screen.
        /// </summary>
        /// <param name="targetRect">目标 RectTransform。/ The target RectTransform.</param>
        /// <param name="side">屏幕的哪一边（顶部、底部、左侧或右侧）。/ The side of the screen (top, bottom, left, or right).</param>
        /// <returns>目标 RectTransform 的外部屏幕位置。/ The outside screen position of the target RectTransform.</returns>
        public static Vector2 CalculateUIOutsideScreenPosition(RectTransform targetRect, Direction side)
        {
            Vector2 position = targetRect.position;

            // 计算锚点中心
            Vector2 anchorMin = targetRect.anchorMin;
            Vector2 anchorMax = targetRect.anchorMax;
            Vector2 anchorCenter = (anchorMax + anchorMin) / 2;

            switch (side)
            {
                case Direction.Up:
                    position = new Vector2(
                        position.x,
                        Screen.height + (anchorCenter.y * targetRect.rect.height));
                    break;
                case Direction.Down:
                    position = new Vector2(
                        position.x,
                        0 - ((1 - anchorCenter.y) * targetRect.rect.height));
                    break;
                case Direction.Left:
                    position = new Vector2(
                        0 - ((1 - anchorCenter.x) * targetRect.rect.width),
                        position.y);
                    break;
                case Direction.Right:
                    position = new Vector2(
                        Screen.width + (anchorCenter.x * targetRect.rect.width),
                        position.y);
                    break;

                default:
                    break;
            }


            return position;

        }

        /// <summary>
        /// 检查 UI 元素是否完全超出屏幕边缘。<br/>
        /// Checks if the UI element is completely out of the screen boundaries.
        /// </summary>
        /// <param name="uiElement">要检查的 UI 元素。/ The UI element to check.</param>
        /// <param name="edgeTolerance">边缘容差，用于定义边缘触发的灵敏度。/ The edge tolerance to define the sensitivity of the edge triggering.</param>
        /// <returns>如果 UI 元素完全在屏幕外，则为 true；否则为 false。/ True if the UI element is completely out of screen; otherwise, false.</returns>
        public static bool IsOutOfScreen(RectTransform uiElement, float edgeTolerance = 0.1f)
        {
            // 确保它处于覆盖模式 / Make sure it is in the overlay mode
            Canvas canvas = uiElement.GetComponentInParent<Canvas>();
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                // 如果画布渲染模式不是 Overlay，记录错误并返回 false
                // If the canvas render mode is not Overlay, log an error and return false
                Dev.Error("The canvas render mode has to be Overlay");
                return false;
            }

            // 获取 UI 元素的四个角在屏幕坐标系中的位置 / Get the screen coordinates of the UI element's four corners
            Vector3[] corners = new Vector3[4];
            uiElement.GetWorldCorners(corners); // This works even in Overlay mode

            // 遍历四个角，检查是否全部在屏幕外 / Iterate through the corners to check if they are all out of screen
            foreach (Vector3 corner in corners)
            {
                // 如果任一角落在屏幕内，返回 false / If any corner is within the screen, return false
                if (corner.x >= edgeTolerance &&
                    corner.x <= Screen.width - edgeTolerance &&
                    corner.y >= edgeTolerance &&
                    corner.y <= Screen.height - edgeTolerance)
                {
                    return false;
                }
            }

            // 如果所有角都不在屏幕内，则返回 true / If all corners are out of the screen, return true
            return true;
        }

        #endregion

    }
}