using UnityEngine;

namespace Mace
{
    public class IntVariableDampingProcessor : VariableBindingProcessor<int, int>, IUpdatableBindingProcessor
    {
        private readonly float damping;
        private readonly bool useUnscaledTime;
        private float dampedValue;
        private float currentVelocity;

        public IntVariableDampingProcessor(BindingInfo bindingInfo, Component viewModel, float damping, bool useUnscaledTime) : base(bindingInfo, viewModel)
        {
            this.damping = damping;
            this.useUnscaledTime = useUnscaledTime;
        }

        public override void Bind()
        {
            base.Bind();
            dampedValue = variableBinding.Property.Value;
            currentVelocity = 0;
        }

        public void Update()
        {
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            dampedValue = Mathf.SmoothDamp(dampedValue, variableBinding.Property.Value, ref currentVelocity, damping, float.MaxValue, dt);
            OnBoundVariableChanged(Mathf.RoundToInt(dampedValue));
        }

        protected override int ProcessValue(int value)
        {
            return value;
        }
    }
}