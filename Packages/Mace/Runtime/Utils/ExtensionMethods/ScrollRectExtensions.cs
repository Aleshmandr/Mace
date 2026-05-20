using UnityEngine;
using UnityEngine.UI;

namespace Mace.Utils
{
    public static class ScrollRectExtensions
    {
        public static void FocusOnChild(this ScrollRect scrollRect, RectTransform child, Vector2 margin)
        {
            scrollRect.FocusOnChild(child, margin, Vector2.zero);
        }

        public static void FocusOnChild(this ScrollRect scrollRect, RectTransform child, Vector2 margin, Vector2 focusPointOffset)
        {
            if (child == null)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();
            scrollRect.StopMovement();

            RectTransform viewport = scrollRect.viewport;
            Rect focusZone = scrollRect.GetFocusZone(margin, focusPointOffset);
            Vector2 focusPoint = focusZone.center;
            Vector2 childCenter = viewport.InverseTransformPoint(child.TransformPoint(child.rect.center));

            Vector2 move = Vector2.zero;

            if (scrollRect.vertical && !IsInside(childCenter.y, focusZone.yMin, focusZone.yMax))
            {
                move.y = childCenter.y - focusPoint.y;
            }

            if (scrollRect.horizontal && !IsInside(childCenter.x, focusZone.xMin, focusZone.xMax))
            {
                move.x = childCenter.x - focusPoint.x;
            }

            // Transform the move vector to world space, then to content local space (in case of scaling or rotation?) and apply it.
            Vector3 worldMove = viewport.TransformDirection(move);
            scrollRect.content.localPosition -= scrollRect.content.InverseTransformDirection(worldMove);

            Vector2 rawNormalizedPosition = scrollRect.normalizedPosition;
            scrollRect.normalizedPosition = new Vector2(Mathf.Clamp01(rawNormalizedPosition.x), Mathf.Clamp01(rawNormalizedPosition.y));
        }

        public static Rect GetFocusZone(this ScrollRect scrollRect, Vector2 margin, Vector2 focusPointOffset)
        {
            Vector2 focusMargin = new Vector2(Mathf.Abs(margin.x), Mathf.Abs(margin.y));
            Vector2 focusPoint = scrollRect.viewport.rect.center + focusPointOffset;
            return Rect.MinMaxRect(
                focusPoint.x - focusMargin.x,
                focusPoint.y - focusMargin.y,
                focusPoint.x + focusMargin.x,
                focusPoint.y + focusMargin.y);
        }

        private static bool IsInside(float value, float min, float max)
        {
            return value >= min && value <= max;
        }
    }
}