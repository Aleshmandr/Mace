using System.Collections.Generic;
using Mace.Pooling;
using Mace.Utils;
using UnityEngine;

namespace Mace
{
    public class SpawnPrefabBinder : ComponentBinder
    {
        [SerializeField] private BindingInfo objectToInstantiate = BindingInfo.Variable<object>();
        [SerializeField] private List<BindableViewModelComponent> prefabs;
        [SerializeField] private Transform parent;
        [SerializeField] private ObjectPool pool;

        private PrefabPicker<BindableViewModelComponent> prefabPicker;
        private BindableViewModelComponent currentItem;
        
        private Transform Parent => parent ? parent : transform;

        protected override void Awake()
        {
            base.Awake();

            RegisterVariable<object>(objectToInstantiate)
                .OnChanged(OnObjectChanged)
                .OnCleared(OnObjectCleared);

            prefabPicker = new PrefabPicker<BindableViewModelComponent>(prefabs);
            currentItem = null;

            FillPool();
        }

        private void FillPool()
        {
            if (pool == null)
            {
                return;
            }
            
            foreach (BindableViewModelComponent prefab in prefabs)
            {
                pool.CreatePool(prefab, 1);
            }
        }

        private void OnObjectChanged(object value)
        {
            BindableViewModelComponent bestPrefab = prefabPicker.FindBestPrefab(value);

            if (bestPrefab)
            {
                if (currentItem == null || bestPrefab.ExpectedType != currentItem.ExpectedType)
                {
                    currentItem = SpawnItem(bestPrefab, Parent);
                }

                currentItem.SetData(value);
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

        private BindableViewModelComponent SpawnItem(BindableViewModelComponent prefab, Transform parent)
        {
            Clear();
            return pool != null ? pool.Spawn(prefab, parent, false) : Instantiate(prefab, parent, false);
        }

        private void Clear()
        {
            if (currentItem != null)
            {
                return;
            }

            if (pool == null)
            {
                Destroy(currentItem);
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