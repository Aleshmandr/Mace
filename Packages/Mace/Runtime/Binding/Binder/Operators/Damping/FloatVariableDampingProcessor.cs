using UnityEngine;

namespace Mace
{
    public class FloatVariableDampingProcessor : VariableBindingProcessor<float, float>, IUpdatableBindingProcessor
    {
        private readonly float damping;
        private readonly bool useUnscaledTime;
        private bool isDampedValueInitialized;
        private float dampedValue;
        private float currentVelocity;
        
        public FloatVariableDampingProcessor(BindingInfo bindingInfo, Component viewModel, float damping, bool useUnscaledTime) : base(bindingInfo, viewModel)
        {
            this.damping = damping;
            this.useUnscaledTime = useUnscaledTime;
        }
        
        public override void Bind()
        {
            base.Bind();
            isDampedValueInitialized = false;
        }

        public void Update()
        {
            if (!isDampedValueInitialized)
            {
                dampedValue = variableBinding.Property.Value;
                currentVelocity = 0f;
                isDampedValueInitialized = true;
            }
            
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            dampedValue = Mathf.SmoothDamp(dampedValue, variableBinding.Property.Value, ref currentVelocity, damping, float.MaxValue, dt);
            OnBoundVariableChanged(dampedValue);
        }

        protected override float ProcessValue(float value)
        {
            return value;
        }
    }
}