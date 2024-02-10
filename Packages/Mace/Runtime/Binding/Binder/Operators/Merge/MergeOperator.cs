﻿using System.Collections.Generic;

namespace Mace
{
	public abstract class MergeOperator<T> : BindingListOperator<T, T>
	{
		protected override T Process(int triggerIndex, IReadOnlyList<T> list)
		{
			return list[triggerIndex];
		}
	}
}
