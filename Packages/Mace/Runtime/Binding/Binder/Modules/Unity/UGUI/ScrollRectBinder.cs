using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Mace.Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mace
{
    [RequireComponent(typeof(ScrollRect))]
    [DefaultExecutionOrder(999)]
    public class ScrollRectBinder : ComponentBinder, IPointerDownHandler, IInitializePotentialDragHandler, IBeginDragHandler, IScrollHandler
    {
        private const float FocusAnimationStopDistance = 0.5f;

        [SerializeField] private BindingInfo focusItem = BindingInfo.Variable<object>();
        [SerializeField] private Vector2 focusMargin;
        [SerializeField] private Vector2 focusPointOffset;
        [SerializeField] private NothingSelectedBehaviorMode nothingSelectedBehaviorMode;
        [SerializeField] private FocusScrollActivationMode focusScrollActivationMode;
        [SerializeField, Min(0f)] private float focusScrollSpeed;
        private ScrollRect scrollRect;
        private Vector2 focusAnimationTargetPosition;
        private Vector2 lastProgrammaticPosition;
        private bool isBindingInitialNotification;
        private bool isFocusAnimationPlaying;
        private bool hasProgrammaticPositionChange;

        private enum NothingSelectedBehaviorMode : byte
        {
            ResetScroll = 0,
            KeepCurrent = 1,
        }

        private enum FocusScrollActivationMode : byte
        {
            FocusOnEnableAndTargetChange = 0,
            FocusOnTargetChangeOnly = 1,
        }

        protected override void Awake()
        {
            base.Awake();
            scrollRect = GetComponent<ScrollRect>();
            RegisterVariable<object>(focusItem).OnChanged(OnItemChanged);
        }

        protected override void OnEnable()
        {
            isBindingInitialNotification = true;
            base.OnEnable();
            isBindingInitialNotification = false;
            scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
        }

        protected override void OnDisable()
        {
            CancelFocusAnimation();
            scrollRect.onValueChanged.RemoveListener(OnScrollRectValueChanged);
            base.OnDisable();
        }

        private void Update()
        {
            if (!isFocusAnimationPlaying)
            {
                return;
            }

            Vector2 currentPosition = scrollRect.content.anchoredPosition;
            float step = 1f - Mathf.Exp(-focusScrollSpeed * Time.deltaTime);
            Vector2 newPosition = Vector2.Lerp(currentPosition, focusAnimationTargetPosition, step);

            if ((newPosition - focusAnimationTargetPosition).sqrMagnitude <= FocusAnimationStopDistance * FocusAnimationStopDistance)
            {
                newPosition = focusAnimationTargetPosition;
                isFocusAnimationPlaying = false;
            }

            SetContentPosition(newPosition);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            CancelFocusAnimation();
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            CancelFocusAnimation();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            CancelFocusAnimation();
        }

        public void OnScroll(PointerEventData eventData)
        {
            CancelFocusAnimation();
        }

        private void OnItemChanged(object itemViewModel)
        {
            if (ShouldSkipInitialFocus())
            {
                return;
            }

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
                    FocusOnChild(child as RectTransform);
                    return true;
                }
            }

            return false;
        }

        private void FocusOnChild(RectTransform child)
        {
            if (child == null)
            {
                return;
            }

            scrollRect.StopMovement();
            Vector2 targetPosition = scrollRect.GetFocusOnChildContentPosition(child, focusMargin, focusPointOffset);
            if (focusScrollSpeed <= 0f)
            {
                CancelFocusAnimation();
                SetContentPosition(targetPosition);
                return;
            }

            focusAnimationTargetPosition = targetPosition;
            isFocusAnimationPlaying = true;
        }

        private void ResetScrollIfNeeded()
        {
            CancelFocusAnimation();
            switch (nothingSelectedBehaviorMode)
            {
                case NothingSelectedBehaviorMode.ResetScroll:
                    SetContentPosition(Vector2.zero);
                    break;
                case NothingSelectedBehaviorMode.KeepCurrent:
                default:
                    // Do nothing
                    break;
            }
        }

        private void OnScrollRectValueChanged(Vector2 value)
        {
            if (hasProgrammaticPositionChange)
            {
                hasProgrammaticPositionChange = false;
                if ((scrollRect.content.anchoredPosition - lastProgrammaticPosition).sqrMagnitude <= 0.01f)
                {
                    return;
                }
            }

            if (isFocusAnimationPlaying)
            {
                CancelFocusAnimation();
            }
        }

        private bool ShouldSkipInitialFocus()
        {
            return isBindingInitialNotification &&
                   focusScrollActivationMode == FocusScrollActivationMode.FocusOnTargetChangeOnly;
        }

        private void CancelFocusAnimation()
        {
            if (!isFocusAnimationPlaying)
            {
                return;
            }

            isFocusAnimationPlaying = false;
            scrollRect.StopMovement();
        }

        private void SetContentPosition(Vector2 position)
        {
            lastProgrammaticPosition = position;
            hasProgrammaticPositionChange = true;
            scrollRect.content.anchoredPosition = position;
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
