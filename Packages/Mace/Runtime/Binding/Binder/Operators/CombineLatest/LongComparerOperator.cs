﻿using System;
using UnityEngine;

namespace Mace
{
	public class LongComparerOperator : Operator
	{
		[SerializeField] private BindingInfo operandA = BindingInfo.Variable<long>();
		[SerializeField] private MathComparisonType operation;
		[SerializeField] private ConstantBindingInfo<long> operandB = new ConstantBindingInfo<long>();

		private long A => operandABinding.Property.GetValue(0);
		private long B => operandBBinding.Property.GetValue(0);

		private ObservableVariable<bool> result;
		private VariableBinding<long> operandABinding;
		private VariableBinding<long> operandBBinding;

		protected override void Awake()
		{
			base.Awake();

			result = new ObservableVariable<bool>();
			ViewModel = new OperatorVariableViewModel<bool>(result);;

			operandABinding = RegisterVariable<long>(operandA).OnChanged(OnOperandChanged).GetBinding();
			operandBBinding = RegisterVariable<long>(operandB).OnChanged(OnOperandChanged).GetBinding();
		}

		protected override Type GetInjectionType()
		{
			return typeof(OperatorVariableViewModel<bool>);
		}

		private void OnOperandChanged(long newValue)
		{
			if (operandABinding.IsBound && operandBBinding.IsBound)
			{
				result.Value = Evaluate();
			}
		}

		private bool Evaluate()
		{
			bool result = false;

			switch (operation)
			{
				case MathComparisonType.Equals: result = A == B; break;
				case MathComparisonType.NotEquals: result = A != B; break;
				case MathComparisonType.Greater: result = A > B; break;
				case MathComparisonType.GreaterOrEquals: result = A >= B; break;
				case MathComparisonType.Less: result = A < B; break;
				case MathComparisonType.LessOrEquals: result = A <= B; break;
			}

			return result;
		}
	}
}
