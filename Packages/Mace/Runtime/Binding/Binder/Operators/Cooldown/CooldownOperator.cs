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

        protected override BindingType[] AllowedTypes => AllowedTypesStatic;

        protected override IBindingProcessor GetBindingProcessor(BindingType bindingType, BindingInfo fromBinding)
        {
            IBindingProcessor result = null;
			
            switch (bindingType)
            {
                case BindingType.Variable:
                    result = new CooldownVariableBindingProcessor<T>(fromBinding, this, cooldown);
                    break;
                case BindingType.Collection:
                    Debug.LogError("Collection cooldown is not supported", this);
                    break;
                case BindingType.Command:
                    result = new CooldownCommandBindingProcessor<T>(fromBinding, this, cooldown);
                    break;
                case BindingType.Event:
                    result = new CooldownEventBindingProcessor<T>(fromBinding, this, cooldown);
                    break;
            }

            return result;
        }
    }
}
