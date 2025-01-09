using UnityEngine;

namespace Mace
{
    public class CooldownEventBindingProcessor<T> : EventBindingProcessor<T, T>
    {
        private readonly float cooldown;
        private float lastRaiseTime;

        public CooldownEventBindingProcessor(BindingInfo bindingInfo, Component viewModel, float cooldown)
            : base(bindingInfo, viewModel)
        {
            this.cooldown = cooldown;
            lastRaiseTime = -cooldown;
        }

        public override void Bind()
        {
            lastRaiseTime = -cooldown;
            base.Bind();
        }

        protected override void BoundEventRaisedHandler(T eventData)
        {
            if (Time.unscaledTime - lastRaiseTime >= cooldown)
            {
                lastRaiseTime = Time.unscaledTime;
                base.BoundEventRaisedHandler(eventData);
            }
        }

        protected override T ProcessValue(T value)
        {
            return value;
        }
    }
}