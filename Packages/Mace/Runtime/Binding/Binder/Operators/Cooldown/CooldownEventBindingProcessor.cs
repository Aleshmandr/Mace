using UnityEngine;

namespace Mace
{
    public class CooldownEventBindingProcessor<T> : EventBindingProcessor<T, T>, IUpdatableBindingProcessor
    {
        private readonly float cooldown;
        private float lastRaiseTime;
        private float pendingValueUpdateTimer;
        private T pendingEventData;

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

        public void Update()
        {
            if (pendingValueUpdateTimer <= 0f)
            {
                return;
            }

            pendingValueUpdateTimer -= Time.unscaledDeltaTime;
            if (pendingValueUpdateTimer <= 0f)
            {
                lastRaiseTime = Time.unscaledTime;
                base.BoundEventRaisedHandler(pendingEventData);
            }
        }

        protected override void BoundEventRaisedHandler(T eventData)
        {
            float passedTime = Time.unscaledTime - lastRaiseTime;
            if (passedTime >= cooldown)
            {
                lastRaiseTime = Time.unscaledTime;
                base.BoundEventRaisedHandler(eventData);
            }
            else
            {
                pendingValueUpdateTimer = cooldown - passedTime;
                pendingEventData = eventData;
            }
        }

        protected override T ProcessValue(T value)
        {
            return value;
        }
    }
}