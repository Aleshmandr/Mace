using System;
using UnityEngine;

namespace Mace
{
    [Serializable]
    public struct EnableBehaviourData
    {
        public Behaviour Target;
        public bool Invert;
    }
}