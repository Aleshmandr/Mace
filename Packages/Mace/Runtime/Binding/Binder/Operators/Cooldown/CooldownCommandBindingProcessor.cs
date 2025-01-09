using UnityEngine;

namespace Mace
{
    public class CooldownCommandBindingProcessor<T> : CommandBindingProcessor<T, T>
    {
        private readonly float cooldown;
        private float lastExecutionRequestTime;

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

        protected override void ProcessedCommandExecuteRequestedHandler(T parameter)
        {
            if (Time.unscaledTime - lastExecutionRequestTime >= cooldown)
            {
                lastExecutionRequestTime = Time.unscaledTime;
                base.ProcessedCommandExecuteRequestedHandler(parameter);
            }
        }

        protected override T ProcessValue(T value)
        {
            return value;
        }
    }
}