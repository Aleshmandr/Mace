using System.Collections.Generic;
using Mace.Pooling;
using UnityEngine;

namespace Mace
{
    public class AmountProgressPrefabSpawner : ComponentBinder
    {
        [SerializeField] private BindingInfo progress = BindingInfo.Variable<int>();
        [SerializeField] private BindingInfo maxValue = BindingInfo.Variable<int>();
        [SerializeField] private Transform container;
        [SerializeField] private GameObject progressPrefab;
        [SerializeField] private GameObject placeholderPrefab;
        [SerializeField] private ObjectPool pool;

        private Transform Container => container ? container : transform;

        private readonly List<GameObject> currentProgressItems = new List<GameObject>();
        private readonly List<GameObject> currentPlaceholderItems = new List<GameObject>();

        private VariableBinding<int> progressBinding;
        private VariableBinding<int> maxValueBinding;

        protected virtual void Reset()
        {
            container = transform;
            pool = GetComponent<ObjectPool>();
        }

        protected override void Awake()
        {
            base.Awake();

            if (pool && progressPrefab)
            {
                pool.CreatePool(progressPrefab, 3);
            }

            if (pool && placeholderPrefab && placeholderPrefab != progressPrefab)
            {
                pool.CreatePool(placeholderPrefab, 3);
            }

            progressBinding = RegisterVariable<int>(progress).OnChanged(OnAmountChanged).OnCleared(Clear).GetBinding();
            maxValueBinding = RegisterVariable<int>(maxValue).OnChanged(OnAmountChanged).OnCleared(Clear).GetBinding();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Clear();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Clear();
        }

        private void OnAmountChanged(int newValue)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (!progressBinding.Property.HasValue || maxValueBinding.IsBound && !maxValueBinding.Property.HasValue)
            {
                Clear();
                return;
            }

            int progressValue = Mathf.Max(0, progressBinding.Property.Value);
            int max = maxValueBinding.Property.HasValue ? Mathf.Max(0, maxValueBinding.Property.Value) : progressValue;

            progressValue = Mathf.Clamp(progressValue, 0, max);

            SetItemsCount(currentProgressItems, progressPrefab, progressValue, true);
            SetItemsCount(currentPlaceholderItems, placeholderPrefab, max - progressValue, false);
        }

        private void SetItemsCount(List<GameObject> items, GameObject prefab, int count, bool spawnFirst)
        {
            if (!prefab)
            {
                Clear(items);
                return;
            }

            while (items.Count < count)
            {
                items.Add(Spawn(prefab, spawnFirst));
            }

            while (items.Count > count)
            {
                DisposeLast(items);
            }
        }

        private void Clear()
        {
            Clear(currentProgressItems);
            Clear(currentPlaceholderItems);
        }

        private void Clear(List<GameObject> items)
        {
            while (items.Count > 0)
            {
                DisposeLast(items);
            }
        }

        private void DisposeLast(List<GameObject> items)
        {
            int index = items.Count - 1;
            Dispose(items[index]);
            items.RemoveAt(index);
        }

        private GameObject Spawn(GameObject prefab, bool spawnFirst)
        {
            GameObject result = pool ? pool.Spawn(prefab, Container, false) : Instantiate(prefab, Container, false);

            if (spawnFirst)
            {
                result.transform.SetSiblingIndex(0);
            }
            else
            {
                result.transform.SetAsLastSibling();
            }

            return result;
        }

        private void Dispose(GameObject item)
        {
            if (!item)
            {
                return;
            }

            if (pool)
            {
                pool.Recycle(item);
            }
            else
            {
                item.SetActive(false);
                Destroy(item);
            }
        }
    }
}