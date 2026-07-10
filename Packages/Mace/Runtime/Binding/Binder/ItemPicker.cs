using System.Collections.Generic;
using Mace.Pooling;
using Mace.Utils;
using UnityEngine;

namespace Mace
{
    public class ItemPicker : MonoBehaviour
    {
        [SerializeField] private List<ViewModelComponent> prefabs;
        [SerializeField] private ObjectPool pool;
        [SerializeField] private bool reportMissingPrefabs = true;

        private PrefabPicker<ViewModelComponent> prefabPicker;
        private bool isInitialized;

        protected void Awake()
        {
            EnsureInitialState();
        }

        public ViewModelComponent SpawnItem(object value, Transform parent)
        {
            EnsureInitialState();

            ViewModelComponent bestPrefab = prefabPicker.FindBestPrefab(value);

            if (bestPrefab != null)
            {
                return SpawnItem(bestPrefab, parent);
            }

            if (reportMissingPrefabs)
            {
                string prettifiedName = value == null ? string.Empty : value.GetType().GetPrettifiedName();
                Debug.LogError($"A suitable prefab could not be found for {value} ({prettifiedName}).");
            }

            return null;
        }

        public ViewModelComponent ReplaceItem(IViewModel oldValue, IViewModel newValue, ViewModelComponent currentItem, Transform parent)
        {
            EnsureInitialState();

            ViewModelComponent result = currentItem;

            var prefabForOldValue = prefabPicker.FindBestPrefab(oldValue);
            var prefabForNewValue = prefabPicker.FindBestPrefab(newValue);

            if (!ReferenceEquals(prefabForOldValue, prefabForNewValue))
            {
                result = SpawnItem(prefabForNewValue, parent);
            }

            return result;
        }

        public void DisposeItem(ViewModelComponent item)
        {
            EnsureInitialState();

            if (item == null)
            {
                return;
            }

            if (pool != null)
            {
                pool.Recycle(item, false);
            }
            else
            {
                // Destroy is deferred, so deactivate the item immediately.
                item.gameObject.SetActive(false);
                Destroy(item.gameObject);
            }
        }

        private void EnsureInitialState()
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;
            prefabPicker = new PrefabPicker<ViewModelComponent>(prefabs);
            FillPool();
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

        private ViewModelComponent SpawnItem(ViewModelComponent bestPrefab, Transform parent)
        {
            if (bestPrefab == null)
            {
                return null;
            }

            ViewModelComponent result;

            if (pool != null)
            {
                result = pool.Spawn(bestPrefab, parent, false);
            }
            else
            {
                result = Instantiate(bestPrefab, parent, false);
                result.gameObject.SetActive(true);
            }

            return result;
        }
    }
}