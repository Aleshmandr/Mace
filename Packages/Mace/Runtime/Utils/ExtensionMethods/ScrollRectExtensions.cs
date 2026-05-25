using UnityEngine;
using UnityEngine.UI;

namespace Mace.Utils
{
    public static class ScrollRectExtensions
    {
        private const float BoundsOffsetThreshold = 0.001f;
        private static readonly Vector3[] Corners = new Vector3[4];

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
            scrollRect.content.anchoredPosition = scrollRect.GetFocusOnChildContentPosition(child, margin, focusPointOffset);
        }

        public static Vector2 GetFocusOnChildContentPosition(this ScrollRect scrollRect, RectTransform child, Vector2 margin,
            Vector2 focusPointOffset)
        {
            if (child == null || scrollRect.content == null)
            {
                return scrollRect.content != null ? scrollRect.content.anchoredPosition : Vector2.zero;
            }

            Canvas.ForceUpdateCanvases();

            RectTransform viewport = GetViewport(scrollRect);
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
            Vector3 contentMove = scrollRect.content.InverseTransformDirection(worldMove);
            Vector2 targetPosition = scrollRect.content.anchoredPosition - new Vector2(contentMove.x, contentMove.y);

            return ClampContentPosition(scrollRect, viewport, targetPosition);
        }

        public static Rect GetFocusZone(this ScrollRect scrollRect, Vector2 margin, Vector2 focusPointOffset)
        {
            RectTransform viewport = GetViewport(scrollRect);
            Vector2 focusMargin = new Vector2(Mathf.Abs(margin.x), Mathf.Abs(margin.y));
            Vector2 focusPoint = viewport.rect.center + focusPointOffset;
            return Rect.MinMaxRect(
                focusPoint.x - focusMargin.x,
                focusPoint.y - focusMargin.y,
                focusPoint.x + focusMargin.x,
                focusPoint.y + focusMargin.y);
        }

        private static RectTransform GetViewport(ScrollRect scrollRect)
        {
            return scrollRect.viewport != null ? scrollRect.viewport : (RectTransform)scrollRect.transform;
        }

        private static Vector2 ClampContentPosition(ScrollRect scrollRect, RectTransform viewport, Vector2 targetPosition)
        {
            if (scrollRect.movementType == ScrollRect.MovementType.Unrestricted)
            {
                return targetPosition;
            }

            RectTransform content = scrollRect.content;
            Bounds viewBounds = new Bounds(viewport.rect.center, viewport.rect.size);
            Bounds contentBounds = GetBounds(content, viewport);

            Vector3 contentSize = contentBounds.size;
            Vector3 contentPos = contentBounds.center;
            Vector2 contentPivot = content.pivot;
            AdjustBounds(ref viewBounds, ref contentPivot, ref contentSize, ref contentPos);
            contentBounds.size = contentSize;
            contentBounds.center = contentPos;

            Vector2 contentDelta = targetPosition - content.anchoredPosition;
            Vector3 worldDelta = content.TransformDirection(contentDelta);
            Vector2 viewDelta = viewport.InverseTransformDirection(worldDelta);
            Vector2 offset = CalculateOffset(viewBounds, contentBounds, scrollRect.horizontal, scrollRect.vertical, viewDelta);

            if (offset != Vector2.zero)
            {
                Vector2 adjustedViewDelta = viewDelta + offset;
                Vector3 adjustedWorldDelta = viewport.TransformDirection(adjustedViewDelta);
                Vector3 adjustedContentDelta = content.InverseTransformDirection(adjustedWorldDelta);
                targetPosition = content.anchoredPosition + new Vector2(adjustedContentDelta.x, adjustedContentDelta.y);
            }

            if (!scrollRect.horizontal)
            {
                targetPosition.x = content.anchoredPosition.x;
            }

            if (!scrollRect.vertical)
            {
                targetPosition.y = content.anchoredPosition.y;
            }

            return targetPosition;
        }

        private static Bounds GetBounds(RectTransform content, RectTransform viewport)
        {
            content.GetWorldCorners(Corners);
            Matrix4x4 viewWorldToLocalMatrix = viewport.worldToLocalMatrix;

            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < Corners.Length; i++)
            {
                Vector3 corner = viewWorldToLocalMatrix.MultiplyPoint3x4(Corners[i]);
                min = Vector3.Min(min, corner);
                max = Vector3.Max(max, corner);
            }

            Bounds bounds = new Bounds(min, Vector3.zero);
            bounds.Encapsulate(max);
            return bounds;
        }

        private static void AdjustBounds(ref Bounds viewBounds, ref Vector2 contentPivot, ref Vector3 contentSize, ref Vector3 contentPos)
        {
            Vector3 excess = viewBounds.size - contentSize;
            if (excess.x > 0)
            {
                contentPos.x -= excess.x * (contentPivot.x - 0.5f);
                contentSize.x = viewBounds.size.x;
            }

            if (excess.y > 0)
            {
                contentPos.y -= excess.y * (contentPivot.y - 0.5f);
                contentSize.y = viewBounds.size.y;
            }
        }

        private static Vector2 CalculateOffset(Bounds viewBounds, Bounds contentBounds, bool horizontal, bool vertical, Vector2 delta)
        {
            Vector2 offset = Vector2.zero;
            Vector2 min = contentBounds.min;
            Vector2 max = contentBounds.max;

            if (horizontal)
            {
                min.x += delta.x;
                max.x += delta.x;

                float maxOffset = viewBounds.max.x - max.x;
                float minOffset = viewBounds.min.x - min.x;

                if (minOffset < -BoundsOffsetThreshold)
                {
                    offset.x = minOffset;
                }
                else if (maxOffset > BoundsOffsetThreshold)
                {
                    offset.x = maxOffset;
                }
            }

            if (vertical)
            {
                min.y += delta.y;
                max.y += delta.y;

                float maxOffset = viewBounds.max.y - max.y;
                float minOffset = viewBounds.min.y - min.y;

                if (maxOffset > BoundsOffsetThreshold)
                {
                    offset.y = maxOffset;
                }
                else if (minOffset < -BoundsOffsetThreshold)
                {
                    offset.y = minOffset;
                }
            }

            return offset;
        }

        private static bool IsInside(float value, float min, float max)
        {
            return value >= min && value <= max;
        }
    }
}