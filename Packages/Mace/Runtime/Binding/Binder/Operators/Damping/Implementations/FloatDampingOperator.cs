using UnityEngine;

namespace Mace
{
    public class FloatDampingOperator : ProcessorOperator<float, float>
    {
        [SerializeField] private float damping = 1f;
        [SerializeField] private bool useUnscaledTime;
        
        protected override IBindingProcessor GetBindingProcessor(BindingType bindingType, BindingInfo fromBinding)
        {
            return new FloatVariableDampingProcessor(fromBinding, this, damping, useUnscaledTime);
        }
    }
}