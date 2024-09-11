using System;
using UnityEngine;

namespace Mace
{
    [Serializable]
    public struct ActivateGameObjectData
    {
        public GameObject Target;
        public bool Invert;
    }
}