using UnityEngine;
using UnityEngine.UI;
using Mace.Utils;
using UnityEditor;

namespace Mace
{
    [RequireComponent(typeof(ScrollRect))]
    [DefaultExecutionOrder(999)]
    public class ScrollRectBinder : ComponentBinder
    {
        [SerializeField] private BindingInfo focusItem = BindingInfo.Variable<object>();
        [SerializeField] private Vector2 focusMargin;
        [SerializeField] private bool resetScrollWhenNothingFocused = true;
        private ScrollRect scrollRect;

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
                    scrollRect.FocusOnChild(child as RectTransform, focusMargin);
                    return true;
                }
            }
            
            return false;
        }

        private void ResetScrollIfNeeded()
        {
            if (!resetScrollWhenNothingFocused)
            {
                return;
            }
            
            scrollRect.content.anchoredPosition = Vector2.zero;
        }

#if UNITY_EDITOR
        [MenuItem("CONTEXT/ScrollRect/Add Binder")]
        private static void AddBinder(MenuCommand command)
        {
            ScrollRect context = (ScrollRect)command.context;
            context.GetOrAddComponent<ScrollRectBinder>();
        }
#endif
    }
}