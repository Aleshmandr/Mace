using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mace.Pooling
{
    public class ObjectPool : MonoBehaviour
    {
        [Serializable]
        private class PrefabInstancesEntry
        {
            public GameObject Prefab;
            public List<GameObject> Instances;

            public PrefabInstancesEntry(GameObject prefab, List<GameObject> instances)
            {
                Prefab = prefab;
                Instances = instances;
            }
        }

        private static readonly string PoolContainerName = "Pool";

        private Dictionary<GameObject, PoolData> CachedPools
        {
            get
            {
                cachedPools ??= new Dictionary<GameObject, PoolData>();
                InitializePools();
                return cachedPools;
            }
        }

        public static ObjectPool Global => SingleObjectPool.Instance.GlobalPool;

        [SerializeField] private int initialPoolSize = 5;
        [SerializeField] private int maxPoolSize = 20;
        [SerializeField] private List<GameObject> poolPrefabs;
        [SerializeField] private List<PrefabInstancesEntry> prefabMap;

        private Transform PoolContainer => GetPoolContainer();

        private bool isInitialized = false;
        private Dictionary<GameObject, PoolData> cachedPools;
        private Transform poolContainer;

        private static bool isApplicationQuitting = false;

        private void OnValidate()
        {
            initialPoolSize = Mathf.Max(initialPoolSize, 0);
            maxPoolSize = Mathf.Max(maxPoolSize, 1);
        }

        private void Awake()
        {
            InitializePools();
        }

        private void OnEnable()
        {
            Application.quitting += OnApplicationQuitting;
        }

        private void OnDisable()
        {
            Application.quitting -= OnApplicationQuitting;
        }

        private static void OnApplicationQuitting()
        {
            isApplicationQuitting = true;
        }

        public void CreatePool(GameObject original, int initialSize)
        {
            CreatePool(original, initialSize, null);
        }

        public void CreatePool<T>(T original, int initialSize) where T : Component
        {
            CreatePool(original.gameObject, initialSize);
        }

        public GameObject Spawn(GameObject original, Transform parent, bool worldPositionStays = true)
        {
            if (isApplicationQuitting)
            {
                Debug.LogWarning($"[ObjectPool] Attempted to spawn '{original.name}' during application quitting.");
                return null;
            }

            GameObject result = null;

            if (Global.CachedPools.TryGetValue(original.gameObject, out PoolData pool))
            {
                result = pool.Spawn(parent, worldPositionStays);
            }

            return result;
        }

        public GameObject Spawn(GameObject original)
        {
            return Spawn(original, transform);
        }

        public T Spawn<T>(T original, Transform parent, bool worldPositionStays = true) where T : Component
        {
            var spawnedObject = Spawn(original.gameObject, parent, worldPositionStays);
            return spawnedObject ? spawnedObject.GetComponent<T>() : null;
        }

        public T Spawn<T>(T original) where T : Component
        {
            var spawnedObject = Spawn(original.gameObject);
            return spawnedObject ? spawnedObject.GetComponent<T>() : null;
        }

        public void Recycle(GameObject item, bool worldPositionStays = true)
        {
            if (isApplicationQuitting)
            {
                Debug.LogWarning($"[ObjectPool] Attempted to recycle '{item.name}' during application quitting.");
                return;
            }

            PoolItem poolItem = item.GetComponent<PoolItem>();

            if (poolItem && Global.CachedPools.TryGetValue(poolItem.Original, out PoolData pool))
            {
                pool.Recycle(poolItem, worldPositionStays);
            }
            else
            {
                Destroy(item);
            }
        }

        public void Recycle<T>(T item, bool worldPositionStays = true) where T : Component
        {
            Recycle(item.gameObject, worldPositionStays);
        }

        [ContextMenu("Prewarm Pools")]
        public void PrewarmPools()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                prefabMap = new List<PrefabInstancesEntry>();

                Transform container = GetPoolContainer();

                foreach (GameObject current in poolPrefabs)
                {
                    if (current)
                    {
                        List<GameObject> prefabInstances = GetExistingPrefabInstances(container, current);
                        AddMissingInstances(prefabInstances, current, container);
                        RemoveExtraInstances(prefabInstances);
                        prefabMap.Add(new PrefabInstancesEntry(current, prefabInstances));
                    }
                }
            }
#endif
        }

        private void InitializePools()
        {
            if (!isInitialized && poolPrefabs != null)
            {
                isInitialized = true;

                foreach (GameObject objectToPool in poolPrefabs)
                {
                    List<GameObject> prewarmedItems = prefabMap
                        ?.Find(x => x.Prefab == objectToPool)
                        ?.Instances;

                    CreatePool(objectToPool, initialPoolSize, prewarmedItems);
                }
            }
        }

        private void CreatePool(GameObject original, int initialSize, IEnumerable<GameObject> prewarmedItems)
        {
            if (isApplicationQuitting)
            {
                Debug.LogWarning($"[ObjectPool] Attempted to create pool for '{original.name}' during application quitting.");
                return;
            }

            if (Global.CachedPools.TryGetValue(original, out PoolData poolData))
            {
                if (prewarmedItems != null)
                {
                    poolData.Merge(maxPoolSize, prewarmedItems);
                }
            }
            else
            {
                Global.CachedPools.Add(original, new PoolData(Global, original, initialSize, 2f, prewarmedItems));
            }
        }

        private Transform GetPoolContainer()
        {
            if (!poolContainer)
            {
                poolContainer = transform.Find(PoolContainerName);

                if (!poolContainer)
                {
                    if (isApplicationQuitting)
                    {
                        Debug.LogWarning("[ObjectPool] Attempted to create pool container during application quitting.");
                        return null;
                    }

                    poolContainer = new GameObject(PoolContainerName).transform;
                    poolContainer.gameObject.SetActive(false);
                    poolContainer.SetParent(transform, false);
                }
            }

            return poolContainer;
        }

#if UNITY_EDITOR
        private List<GameObject> GetExistingPrefabInstances(Transform container, GameObject current)
        {
            List<GameObject> result = new List<GameObject>();

            foreach (Transform child in container)
            {
                GameObject correspondingObject = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);

                if (Equals(correspondingObject, current))
                {
                    result.Add(child.gameObject);
                }
            }

            return result;
        }

        private void AddMissingInstances(ICollection<GameObject> prefabInstances, GameObject current, Transform container)
        {
            for (int i = prefabInstances.Count; i < initialPoolSize; i++)
            {
                if (isApplicationQuitting)
                {
                    Debug.LogWarning($"[ObjectPool] Attempted to instantiate '{current.name}' during application quitting.");
                    break;
                }

                prefabInstances.Add(PrefabUtility.InstantiatePrefab(current, container) as GameObject);
            }
        }

        private void RemoveExtraInstances(IList<GameObject> prefabInstances)
        {
            while (prefabInstances.Count > 0 && prefabInstances.Count > initialPoolSize)
            {
                int lastIndex = prefabInstances.Count - 1;
                GameObject instance = prefabInstances[lastIndex];
                prefabInstances.RemoveAt(lastIndex);
                DestroyImmediate(instance);
            }
        }
#endif
    }
}
