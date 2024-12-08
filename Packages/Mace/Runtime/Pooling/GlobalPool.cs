using System.Collections.Generic;
using UnityEngine;

namespace Mace.Pooling
{
    internal class GlobalPool : ObjectPool
    {
        private readonly Queue<Transform> transformsToReparent = new();

        public Dictionary<GameObject, PoolData> CachedPools { get; } = new ();

        private void Update()
        {
            while (transformsToReparent.Count > 0)
            {
                Transform t = transformsToReparent.Dequeue();
                t.SetParent(transform, false);
            }
        }

        public override void EnqueueReparent(Transform t)
        {
            transformsToReparent.Enqueue(t);
        }
    }
}