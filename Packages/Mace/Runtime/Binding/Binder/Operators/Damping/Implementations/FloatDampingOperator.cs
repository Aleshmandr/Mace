using UnityEngine;

namespace Mace
{
    public class FloatDampingOperator : ProcessorOperator<float, float>
    {
        [SerializeField] private float damping = 1f;
        [SerializeField] private bool useUnscaledTime;
        private FloatVariableDampingProcessor dampingProcessor;
        
        protected override IBindingProcessor GetBindingProcessor(BindingType bindingType, BindingInfo fromBinding)
        {
            return new FloatVariableDampingProcessor(fromBinding, this, damping, useUnscaledTime);
        }
        
        private void Update()
        {
            dampingProcessor?.Update();
        }
    }
}