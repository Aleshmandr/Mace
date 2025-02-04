using System;
using UnityEngine;

namespace Mace
{
    public class CooldownOperator<T> : ProcessorOperator<T, T>
    {
        private static readonly BindingType[] AllowedTypesStatic =
        {
            BindingType.Variable,
            BindingType.Command,
            BindingType.Event
        };

        [SerializeField] private float cooldown = 1f;
        private IUpdatableBindingProcessor updatableBindingProcessor;

        protected override BindingType[] AllowedTypes => AllowedTypesStatic;

        protected override IBindingProcessor GetBindingProcessor(BindingType bindingType, BindingInfo fromBinding)
        {
            switch (bindingType)
            {
                case BindingType.Variable:
                    var cooldownVariableBindingProcessor = new CooldownVariableBindingProcessor<T>(fromBinding, this, cooldown);
                    updatableBindingProcessor = cooldownVariableBindingProcessor;
                    break;
                case BindingType.Collection:
                    updatableBindingProcessor = null;
                    Debug.LogError("Collection cooldown is not supported", this);
                    break;
                case BindingType.Command:
                    updatableBindingProcessor = new CooldownCommandBindingProcessor<T>(fromBinding, this, cooldown);
                    break;
                case BindingType.Event:
                    updatableBindingProcessor = new CooldownEventBindingProcessor<T>(fromBinding, this, cooldown);
                    break;
            }

            return updatableBindingProcessor;
        }

        private void Update()
        {
            updatableBindingProcessor?.Update();
        }
    }
}