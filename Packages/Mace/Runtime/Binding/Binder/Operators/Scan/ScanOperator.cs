﻿namespace Mace
{
	public abstract class ScanOperator<T> : ProcessorOperator<T, T>
	{
		protected abstract T Scan(T value, T accumulatedValue);

		protected abstract T GetInitialAccumulatedValue();

		protected override IBindingProcessor GetBindingProcessor(BindingType bindingType, BindingInfo fromBinding)
		{
			IBindingProcessor result = null;
			
			switch (bindingType)
			{
				case BindingType.Variable:
					result = new ScanVariableBindingProcessor<T>(fromBinding, this, Scan, GetInitialAccumulatedValue);
					break;
				case BindingType.Collection:
					result = new ScanCollectionBindingProcessor<T>(fromBinding, this, Scan, GetInitialAccumulatedValue);
					break;
				case BindingType.Command:
					result = new ScanCommandBindingProcessor<T>(fromBinding, this, Scan, GetInitialAccumulatedValue);
					break;
				case BindingType.Event:
					result = new ScanEventBindingProcessor<T>(fromBinding, this, Scan, GetInitialAccumulatedValue);
					break;
			}

			return result;
		}
	}
}