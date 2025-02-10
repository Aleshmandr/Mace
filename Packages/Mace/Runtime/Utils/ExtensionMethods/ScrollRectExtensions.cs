using UnityEngine;
using UnityEngine.UI;

namespace Mace.Utils
{
    public static class ScrollRectExtensions
    {
        public static void FocusOnChild(this ScrollRect scrollRect, RectTransform child, Vector2 margin)
        {
            if (child == null)
            {
                return;
            }
            
            Canvas.ForceUpdateCanvases();

            // Get min and max of the viewport and child in local space to the viewport so we can compare them.
            // NOTE: use viewport instead of the scrollRect as viewport doesn't include the scrollbars in it.
            Vector2 viewPosMin = scrollRect.viewport.rect.min;
            Vector2 viewPosMax = scrollRect.viewport.rect.max;

            Vector2 childPosMin = scrollRect.viewport.InverseTransformPoint(child.TransformPoint(child.rect.min));
            Vector2 childPosMax = scrollRect.viewport.InverseTransformPoint(child.TransformPoint(child.rect.max));

            childPosMin -= margin;
            childPosMax += margin;

            Vector2 move = Vector2.zero;

            // Check if one (or more) of the child bounding edges goes outside the viewport and
            // calculate move vector for the content rect so it can keep it visible.
            if (childPosMax.y > viewPosMax.y)
            {
                move.y = childPosMax.y - viewPosMax.y;
            }

            if (childPosMin.x < viewPosMin.x)
            {
                move.x = childPosMin.x - viewPosMin.x;
            }

            if (childPosMax.x > viewPosMax.x)
            {
                move.x = childPosMax.x - viewPosMax.x;
            }

            if (childPosMin.y < viewPosMin.y)
            {
                move.y = childPosMin.y - viewPosMin.y;
            }

            // Transform the move vector to world space, then to content local space (in case of scaling or rotation?) and apply it.
            Vector3 worldMove = scrollRect.viewport.TransformDirection(move);
            scrollRect.content.localPosition -= scrollRect.content.InverseTransformDirection(worldMove);
        }
    }
}