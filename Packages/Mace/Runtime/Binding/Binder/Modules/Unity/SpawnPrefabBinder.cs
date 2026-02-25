using UnityEngine;
using UnityEngine.Assertions;

namespace Mace
{
    public class SpawnPrefabBinder : ComponentBinder
    {
        [SerializeField] private BindingInfo objectToInstantiate = BindingInfo.Variable<object>();
        [SerializeField] private bool keepBindingActiveWhileDisabled;
        [SerializeField] private Transform itemContainer;
        [Header("Dependencies")]
        [SerializeField] protected ItemPicker itemPicker;
        private ViewModelComponent currentItem;

        private Transform Container => itemContainer ? itemContainer : transform;

        protected override void Awake()
        {
            base.Awake();

            Assert.IsNotNull(itemPicker, $"A {nameof(SpawnPrefabBinder)} needs an {nameof(ItemPicker)} to work.");
            
            RegisterVariable<object>(objectToInstantiate).OnChanged(OnObjectChanged).OnCleared(OnObjectCleared);

            if (keepBindingActiveWhileDisabled)
            {
                Bind();
            }
        }

        protected override void OnEnable()
        {
            if (keepBindingActiveWhileDisabled)
            {
                return;
            }

            Bind();
        }

        protected override void OnDisable()
        {
            if (keepBindingActiveWhileDisabled)
            {
                return;
            }

            Unbind();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (keepBindingActiveWhileDisabled)
            {
                Unbind();
            }
        }

        private void OnObjectChanged(object value)
        {
            Clear();
            currentItem = itemPicker.SpawnItem(value, Container);

            if (currentItem == null)
            {
                return;
            }
            
            currentItem.ViewModel = value as ViewModel;
        }

        private void OnObjectCleared()
        {
            Clear();
        }

        private void Clear()
        {
            itemPicker.DisposeItem(currentItem);
            currentItem = null;
        }
        
        protected virtual void OnValidate()
        {
            if (itemContainer != null)
            {
                return;
            }

            itemContainer = transform;
        }
    }
}