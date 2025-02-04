using UnityEngine;

namespace Mace
{
    public class CooldownCommandBindingProcessor<T> : CommandBindingProcessor<T, T>, IUpdatableBindingProcessor
    {
        private readonly float cooldown;
        private float lastExecutionRequestTime;
        private float pendingValueUpdateTimer;
        private T pendingParameter;

        public CooldownCommandBindingProcessor(BindingInfo bindingInfo, Component viewModel, float cooldown)
            : base(bindingInfo, viewModel)
        {
            this.cooldown = cooldown;
            lastExecutionRequestTime = -cooldown;
        }

        public override void Bind()
        {
            lastExecutionRequestTime = -cooldown;
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
                lastExecutionRequestTime = Time.unscaledTime;
                base.ProcessedCommandExecuteRequestedHandler(pendingParameter);
            }
        }

        protected override void ProcessedCommandExecuteRequestedHandler(T parameter)
        {
            float passedTime = Time.unscaledTime - lastExecutionRequestTime;
            if (passedTime >= cooldown)
            {
                lastExecutionRequestTime = Time.unscaledTime;
                base.ProcessedCommandExecuteRequestedHandler(parameter);
            }
            else
            {
                pendingValueUpdateTimer = cooldown - passedTime;
                pendingParameter = parameter;
            }
        }

        protected override T ProcessValue(T value)
        {
            return value;
        }
    }
}