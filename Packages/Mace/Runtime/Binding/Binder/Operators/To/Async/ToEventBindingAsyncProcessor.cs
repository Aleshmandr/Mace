﻿using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Mace
{
	public class ToEventBindingAsyncProcessor<TFrom, TTo> : EventBindingAsyncProcessor<TFrom, TTo>
	{
		private readonly Func<TFrom, Task<TTo>> processFunction;

		public ToEventBindingAsyncProcessor(BindingInfo bindingInfo, Component viewModel, Func<TFrom, Task<TTo>> processFunction)
			: base(bindingInfo, viewModel)
		{
			this.processFunction = processFunction;
		}

		protected override async Task<TTo> ProcessValueAsync(TFrom value)
		{
			return await processFunction(value);
		}
	}
}