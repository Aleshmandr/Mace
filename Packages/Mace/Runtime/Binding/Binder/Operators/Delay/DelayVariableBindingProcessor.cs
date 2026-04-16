using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mace
{
    public class DelayVariableBindingProcessor<T> : VariableBindingProcessor<T, T>, IUpdatableBindingProcessor
    {
        private readonly Queue<ActionCommand> actionQueue;
        private readonly float delay;
        private readonly bool syncOnBind;

        public DelayVariableBindingProcessor(BindingInfo bindingInfo, Component viewModel, float delay, bool syncOnBind) : base(bindingInfo, viewModel)
        {
            actionQueue = new Queue<ActionCommand>();
            this.delay = delay;
            this.syncOnBind = syncOnBind;
        }

        public override void Unbind()
        {
            actionQueue.Clear();
            base.Unbind();
        }

        protected override void OnBoundVariableChanged(T newValue)
        {
            if (syncOnBind && !processedVariable.HasValue)
            {
                base.OnBoundVariableChanged(newValue);
                return;
            }
            
            EnqueueAction(() => base.OnBoundVariableChanged(newValue));
        }

        protected override T ProcessValue(T value)
        {
            return value;
        }

        public void Update()
        {
            while (actionQueue.Count > 0 && Time.realtimeSinceStartup >= actionQueue.Peek().DueTime)
            {
                actionQueue.Dequeue().Execute();
            }
        }

        private void EnqueueAction(Action action)
        {
            actionQueue.Enqueue(new ActionCommand(action, Time.realtimeSinceStartup + delay));
        }
    }
}