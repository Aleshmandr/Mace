﻿using UnityEngine;

namespace Mace
{
	public class SkipVariableBindingProcessor<T> : VariableBindingProcessor<T, T>
	{
		private readonly int skipAmount;
		private int valueReceivedCount;
		
		public SkipVariableBindingProcessor(BindingInfo bindingInfo, Component viewModel, int skipAmount)
			: base(bindingInfo, viewModel)
		{
			this.skipAmount = skipAmount;
			valueReceivedCount = 0;
		}

		public override void Bind()
		{
			valueReceivedCount = 0;
			
			base.Bind();
		}

		protected override void OnBoundVariableChanged(T newValue)
		{
			valueReceivedCount++;
			
			if (valueReceivedCount >= skipAmount)
			{
				base.OnBoundVariableChanged(newValue);
			}
		}

		protected override T ProcessValue(T value)
		{
			return value;
		}
	}
}