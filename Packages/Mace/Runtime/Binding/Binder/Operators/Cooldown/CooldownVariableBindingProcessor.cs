using UnityEngine;

namespace Mace
{
    public class CooldownVariableBindingProcessor<T> : VariableBindingProcessor<T, T>, IUpdatableBindingProcessor
    {
        private readonly float cooldown;
        private float lastChangeTime;
        private float pendingValueUpdateTimer;
        private T pendingValue;

        public CooldownVariableBindingProcessor(BindingInfo bindingInfo, Component viewModel, float cooldown)
            : base(bindingInfo, viewModel)
        {
            this.cooldown = cooldown;
            lastChangeTime = -cooldown;
        }

        public override void Bind()
        {
            lastChangeTime = -cooldown;
            base.Bind();
        }

        public void Update()
        {
            if (pendingValueUpdateTimer <= 0f)
            {
                return;
            }

            pendingValueUpdateTimer -= Time.unscaledDeltaTime;
            if (pendingValueUpdateTimer <= 0f)
            {
                lastChangeTime = Time.unscaledTime;
                base.OnBoundVariableChanged(pendingValue);
            }
        }

        protected override void OnBoundVariableChanged(T newValue)
        {
            float passedTime = Time.unscaledTime - lastChangeTime;
            if (passedTime >= cooldown)
            {
                lastChangeTime = Time.unscaledTime;
                pendingValueUpdateTimer = 0f;
                base.OnBoundVariableChanged(newValue);
            }
            else
            {
                pendingValueUpdateTimer = cooldown - passedTime;
                pendingValue = newValue;
            }
        }

        protected override T ProcessValue(T value)
        {
            return value;
        }
    }
}