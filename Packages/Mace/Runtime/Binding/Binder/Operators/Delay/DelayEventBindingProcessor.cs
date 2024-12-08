using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mace
{
	public class DelayEventBindingProcessor<T> : EventBindingProcessor<T, T>, IUpdatableBindingProcessor
	{
		private Queue<ActionCommand> actionQueue;
		private float delay;
		
		public DelayEventBindingProcessor(BindingInfo bindingInfo, Component viewModel, float delay)
			: base(bindingInfo, viewModel)
		{
			actionQueue = new Queue<ActionCommand>();
			this.delay = delay;
		}

		public override void Unbind()
		{
			actionQueue.Clear();
			base.Unbind();
		}

		protected override T ProcessValue(T value)
		{
			return value;
		}

		protected override void BoundEventRaisedHandler(T eventData)
		{
			EnqueueAction(() => base.BoundEventRaisedHandler(eventData));
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