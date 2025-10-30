using System.Collections.Generic;
using Mace.Pooling;
using Mace.Utils;
using UnityEngine;

namespace Mace
{
    public class SpawnPrefabBinder : ComponentBinder
    {
        [SerializeField] private BindingInfo objectToInstantiate = BindingInfo.Variable<object>();
        [SerializeField] private bool keepBindingActiveWhileDisabled;
        [SerializeField] private List<ViewModelComponent> prefabs;
        [SerializeField] private Transform parent;
        [SerializeField] private ObjectPool pool;

        private PrefabPicker<ViewModelComponent> prefabPicker;
        private ViewModelComponent currentItem;
        private bool isInitialized;

        private Transform Parent => parent ? parent : transform;

        protected override void Awake()
        {
            base.Awake();

            RegisterVariable<object>(objectToInstantiate)
                .OnChanged(OnObjectChanged)
                .OnCleared(OnObjectCleared);

            prefabPicker = new PrefabPicker<ViewModelComponent>(prefabs);
            currentItem = null;

            FillPool();

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

        private void FillPool()
        {
            if (pool == null)
            {
                return;
            }

            foreach (ViewModelComponent prefab in prefabs)
            {
                pool.CreatePool(prefab, 1);
            }
        }

        private void OnObjectChanged(object value)
        {
            if (value == null)
            {
                Clear();
                return;
            }
            
            ViewModelComponent bestPrefab = prefabPicker.FindBestPrefab(value);

            if (bestPrefab)
            {
                if (currentItem == null || bestPrefab.ExpectedType != currentItem.ExpectedType)
                {
                    currentItem = SpawnItem(bestPrefab, Parent);
                }

                currentItem.ViewModel = (IViewModel)value;
            }
            else
            {
                Clear();
                Debug.LogError($"A matching prefab could not be found for {value} ({value.GetType().GetPrettifiedName()})");
            }
        }

        private void OnObjectCleared()
        {
            Clear();
        }

        private ViewModelComponent SpawnItem(ViewModelComponent prefab, Transform parent)
        {
            Clear();
            return pool != null ? pool.Spawn(prefab, parent, false) : Instantiate(prefab, parent, false);
        }

        private void Clear()
        {
            if (currentItem == null)
            {
                return;
            }

            if (pool == null)
            {
                Destroy(currentItem.gameObject);
            }
            else
            {
                pool.Recycle(currentItem);
            }

            currentItem = null;
        }


#if UNITY_EDITOR
        protected void Reset()
        {
            pool = GetComponent<ObjectPool>();
            if (parent == null)
            {
                parent = transform;
            }
        }
#endif
    }
}