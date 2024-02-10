﻿using System;
using UnityEngine;

namespace Mace
{
	public class ScanCollectionBindingProcessor<T> : CollectionBindingProcessor<T, T>
	{
		private readonly Func<T, T, T> scanFunction;
		private readonly Func<T> initialAccumulatedValueGetter;

		public ScanCollectionBindingProcessor(
			BindingInfo bindingInfo,
			Component viewModel,
			Func<T, T, T> scanFunction,
			Func<T> initialAccumulatedValueGetter)
			: base(bindingInfo, viewModel)
		{
			this.scanFunction = scanFunction;
			this.initialAccumulatedValueGetter = initialAccumulatedValueGetter;
		}

		protected override T ProcessValue(T newValue, T oldValue, bool isNewItem)
		{
			return scanFunction(newValue, oldValue);
		}

		protected override void OnBoundCollectionItemAdded(int index, T value)
		{
			T processedValue = ProcessValue(value, initialAccumulatedValueGetter(), true);
			processedCollection.Insert(index, processedValue);
		}

		protected override void OnBoundCollectionItemReplaced(int index, T oldValue, T newValue)
		{
			T processedValue = ProcessValue(newValue, oldValue, false);
			processedCollection[index] = processedValue;
		}
	}
}