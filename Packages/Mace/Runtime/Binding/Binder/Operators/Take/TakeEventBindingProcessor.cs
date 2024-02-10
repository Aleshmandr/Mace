﻿using UnityEngine;

namespace Mace
{
	public class TakeEventBindingProcessor<T> : EventBindingProcessor<T, T>
	{
		private readonly int takeAmount;
		private int valueReceivedCount;
		
		public TakeEventBindingProcessor(BindingInfo bindingInfo, Component viewModel, int takeAmount)
			: base(bindingInfo, viewModel)
		{
			this.takeAmount = takeAmount;
			valueReceivedCount = 0;
		}

		public override void Bind()
		{
			valueReceivedCount = 0;
			
			base.Bind();
		}

		protected override void BoundEventRaisedHandler(T eventData)
		{
			valueReceivedCount++;
			
			if (valueReceivedCount <= takeAmount)
			{
				base.BoundEventRaisedHandler(eventData);
			}
		}

		protected override T ProcessValue(T value)
		{
			return value;
		}
	}
}