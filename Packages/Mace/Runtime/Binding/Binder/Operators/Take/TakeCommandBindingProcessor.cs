﻿using UnityEngine;

namespace Mace
{
	public class TakeCommandBindingProcessor<T> : CommandBindingProcessor<T, T>
	{
		private readonly int takeAmount;
		private int valueReceivedCount;
		
		public TakeCommandBindingProcessor(BindingInfo bindingInfo, Component viewModel, int takeAmount)
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

		protected override void ProcessedCommandExecuteRequestedHandler(T parameter)
		{
			valueReceivedCount++;
			
			if (valueReceivedCount <= takeAmount)
			{
				base.ProcessedCommandExecuteRequestedHandler(parameter);
			}
		}
		
		protected override T ProcessValue(T value)
		{
			return value;
		}
	}
}