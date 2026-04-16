using UnityEngine;

namespace Mace
{
    public class IntVariableDampingProcessor : VariableBindingProcessor<int, int>, IUpdatableBindingProcessor
    {
        private readonly float damping;
        private readonly bool useUnscaledTime;
        private float dampedValue;
        private int targetValue;
        private float currentVelocity;
        private bool isDampedValueInitialized;

        public IntVariableDampingProcessor(BindingInfo bindingInfo, Component viewModel, float damping, bool useUnscaledTime) : base(bindingInfo, viewModel)
        {
            this.damping = damping;
            this.useUnscaledTime = useUnscaledTime;
        }

        public override void Bind()
        {
            base.Bind();
            isDampedValueInitialized = false;
        }

        protected override void OnBoundVariableChanged(int newValue)
        {
            targetValue = newValue;
        }

        public void Update()
        {
            if (targetValue == processedVariable.Value && isDampedValueInitialized)
            {
                return;
            }

            if (!isDampedValueInitialized)
            {
                dampedValue = targetValue = variableBinding.Property.Value;
                currentVelocity = 0;
                isDampedValueInitialized = true;
            }

            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            dampedValue = Mathf.SmoothDamp(dampedValue, targetValue, ref currentVelocity, damping, float.MaxValue, dt);
            base.OnBoundVariableChanged(Mathf.RoundToInt(dampedValue));
        }

        protected override int ProcessValue(int value)
        {
            return value;
        }
    }
}