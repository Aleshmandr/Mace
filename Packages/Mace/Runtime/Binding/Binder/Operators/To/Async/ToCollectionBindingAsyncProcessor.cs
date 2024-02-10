﻿using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Mace
{
	public class ToCollectionBindingAsyncProcessor<TFrom, TTo> : CollectionBindingAsyncProcessor<TFrom, TTo>
	{
		private readonly Func<TFrom, Task<TTo>> processFunction;

		public ToCollectionBindingAsyncProcessor(BindingInfo bindingInfo, Component viewModel, Func<TFrom, Task<TTo>> processFunction)
			: base(bindingInfo, viewModel)
		{
			this.processFunction = processFunction;
		}

		protected override async Task<TTo> ProcessValueAsync(TFrom newValue, TFrom oldValue, bool isNewItem)
		{
			return await processFunction(newValue);
		}
	}
}