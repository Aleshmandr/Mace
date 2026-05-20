using UnityEngine;
using UnityEngine.UI;
using Mace.Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mace
{
    [RequireComponent(typeof(ScrollRect))]
    [DefaultExecutionOrder(999)]
    public class ScrollRectBinder : ComponentBinder
    {
        [SerializeField] private BindingInfo focusItem = BindingInfo.Variable<object>();
        [SerializeField] private Vector2 focusMargin;
        [SerializeField] private Vector2 focusPointOffset;
        [SerializeField] private NothingSelectedBehaviorMode nothingSelectedBehaviorMode;
        private ScrollRect scrollRect;

        private enum NothingSelectedBehaviorMode : byte
        {
            ResetScroll = 0,
            KeepCurrent = 1,
        }

        protected override void Awake()
        {
            base.Awake();
            scrollRect = GetComponent<ScrollRect>();
            RegisterVariable<object>(focusItem).OnChanged(OnItemChanged);
        }

        private void OnItemChanged(object itemViewModel)
        {
            if (!TryFocusOnItem(itemViewModel))
            {
                ResetScrollIfNeeded();
            }
        }

        private bool TryFocusOnItem(object itemViewModel)
        {
            if (itemViewModel == null)
            {
                return false;
            }

            RectTransform parentTransform = scrollRect.content;
            int childCount = parentTransform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = parentTransform.GetChild(i);
                ViewModelComponent viewModelComponent = child.GetComponent<ViewModelComponent>();
                if (viewModelComponent != null && viewModelComponent.ViewModel == itemViewModel)
                {
                    scrollRect.FocusOnChild(child as RectTransform, focusMargin, focusPointOffset);
                    return true;
                }
            }

            return false;
        }

        private void ResetScrollIfNeeded()
        {
            switch (nothingSelectedBehaviorMode)
            {
                case NothingSelectedBehaviorMode.ResetScroll:
                    scrollRect.content.anchoredPosition = Vector2.zero;
                    break;
                case NothingSelectedBehaviorMode.KeepCurrent:
                default:
                    // Do nothing
                    break;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            ScrollRect targetScrollRect = scrollRect != null ? scrollRect : GetComponent<ScrollRect>();
            if (targetScrollRect == null || targetScrollRect.viewport == null)
            {
                return;
            }

            Rect focusZone = targetScrollRect.GetFocusZone(focusMargin, focusPointOffset);
            Vector3 bottomLeft = new Vector3(focusZone.xMin, focusZone.yMin, 0f);
            Vector3 topLeft = new Vector3(focusZone.xMin, focusZone.yMax, 0f);
            Vector3 topRight = new Vector3(focusZone.xMax, focusZone.yMax, 0f);
            Vector3 bottomRight = new Vector3(focusZone.xMax, focusZone.yMin, 0f);

            Color color = Gizmos.color;
            Matrix4x4 matrix = Gizmos.matrix;

            Gizmos.color = Color.white;
            Gizmos.matrix = targetScrollRect.viewport.localToWorldMatrix;
            Gizmos.DrawLine(bottomLeft, topLeft);
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            DrawFocusPointGizmo(focusZone.center, targetScrollRect.viewport.rect);

            Gizmos.matrix = matrix;
            Gizmos.color = color;
        }

        private static void DrawFocusPointGizmo(Vector2 center, Rect viewportRect)
        {
            float size = Mathf.Max(5f, Mathf.Min(viewportRect.width, viewportRect.height) * 0.03f);
            Vector3 left = new Vector3(center.x - size, center.y, 0f);
            Vector3 right = new Vector3(center.x + size, center.y, 0f);
            Vector3 bottom = new Vector3(center.x, center.y - size, 0f);
            Vector3 top = new Vector3(center.x, center.y + size, 0f);

            Gizmos.DrawLine(left, right);
            Gizmos.DrawLine(bottom, top);
        }

        [MenuItem("CONTEXT/ScrollRect/Add Binder")]
        private static void AddBinder(MenuCommand command)
        {
            ScrollRect context = (ScrollRect)command.context;
            context.GetOrAddComponent<ScrollRectBinder>();
        }
#endif
    }
}