using UnityEngine;

namespace Mace.Utils
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static readonly object lockObject = new object();
        private static T instance;
        private static bool isApplicationQuitting = false;

        static Singleton()
        {
            Application.quitting += OnApplicationQuitting;
        }

        private static void OnApplicationQuitting()
        {
            isApplicationQuitting = true;
        }

        public static T Instance
        {
            get
            {
                if (isApplicationQuitting)
                {
                    Debug.LogWarning($"[Mace] Instance of {typeof(T)} already destroyed on application quit. Won't create again - returning null.");
                    return null;
                }

                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = FindObjectOfType<T>();

                        if (instance == null)
                        {
                            GameObject go = new GameObject($"[Mace] {typeof(T).Name}");
                            instance = go.AddComponent<T>();
                            DontDestroyOnLoad(go);
                        }
                    }

                    return instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}