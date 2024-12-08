﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mace
{
	public class DelayCollectionBindingProcessor<T> : CollectionBindingProcessor<T, T>, IUpdatableBindingProcessor
	{
		private readonly Queue<ActionCommand> actionQueue;
		private readonly float delay;
		
		public DelayCollectionBindingProcessor(BindingInfo bindingInfo, Component viewModel, float delay)
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

		protected override void OnBoundCollectionReset()
		{
			EnqueueAction(() => base.OnBoundCollectionReset());
		}

		protected override void OnBoundCollectionItemAdded(int index, T value)
		{
			EnqueueAction(() => base.OnBoundCollectionItemAdded(index, value));
		}

		protected override void OnBoundCollectionItemMoved(int oldIndex, int newIndex, T value)
		{
			EnqueueAction(() => base.OnBoundCollectionItemMoved(oldIndex, newIndex, value));
		}

		protected override void OnBoundCollectionItemRemoved(int index, T value)
		{
			EnqueueAction(() => base.OnBoundCollectionItemRemoved(index, value));
		}

		protected override void OnBoundCollectionItemReplaced(int index, T oldValue, T newValue)
		{
			EnqueueAction(() => base.OnBoundCollectionItemReplaced(index, oldValue, newValue));
		}

		protected override T ProcessValue(T newValue, T oldValue, bool isNewItem)
		{
			return newValue;
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