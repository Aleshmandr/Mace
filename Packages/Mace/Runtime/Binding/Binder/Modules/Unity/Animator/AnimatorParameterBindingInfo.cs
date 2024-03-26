using System;
using UnityEngine;

namespace Mace
{
    [Serializable]
    public class AnimatorParameterBindingInfo<T>
    {
        [SerializeField] protected string name;
        [SerializeField] protected BindingInfo binding = BindingInfo.Variable<T>();
        private bool isHashGenerated;
        private int hash;

        public BindingInfo Binding => binding;
        public string Name => name;

        public int Id
        {
            get
            {
                if (isHashGenerated)
                {
                    return hash;
                }
                hash = Animator.StringToHash(name);
                isHashGenerated = true;
                return hash;
            }
        }

        // Workaround: Default values for serializable class not supported in Unity
        public void ValidateBinding()
        {
            if (binding == null || binding.Type == null)
            {
                binding = BindingInfo.Variable<T>();
            }
        }
    }
}