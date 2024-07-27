using System;
using System.Linq;
using Mace.Utils;
using UnityEngine;

namespace Mace
{
	public class StringFormatOperator : ViewModelComponent, IViewModelInjector
	{
		public Type InjectionType => typeof(OperatorVariableViewModel<string>);
		public ViewModelComponent Target => this;

		[SerializeField] private ConstantBindingInfo<string> format = new ConstantBindingInfo<string>();
		[SerializeField] private BindingInfoList parameters = new BindingInfoList(typeof(IReadOnlyObservableVariable<object>));

		private VariableBinding<string> formatBinding;
		private BindingList<object> parametersBindingList;
		private ObservableVariable<string> exposedProperty;

		protected override void Awake()
		{
			base.Awake();

			exposedProperty = new ObservableVariable<string>();
			ViewModel = new OperatorVariableViewModel<string>(exposedProperty);

			formatBinding = new VariableBinding<string>(format, this);
			formatBinding.Property.Changed += OnFormatChanged;

			parametersBindingList = new BindingList<object>(this, parameters);
			parametersBindingList.VariableChanged += OnBindingListVariableChanged;
		}

		protected virtual void OnEnable()
		{
			formatBinding.Bind();
			parametersBindingList.Bind();
		}

		protected virtual void OnDisable()
		{
			formatBinding.Unbind();
			parametersBindingList.Unbind();
			exposedProperty.Clear();
		}

		private void OnFormatChanged(string newValue)
		{
			RefreshExposedValue();
		}

		private void OnBindingListVariableChanged(int index, object newValue)
		{
			RefreshExposedValue();
		}

		private void RefreshExposedValue()
		{
			if (formatBinding.IsBound && formatBinding.Property.HasValue && !string.IsNullOrEmpty(formatBinding.Property.Value))
			{
				exposedProperty.Value = string.Format(formatBinding.Property.Value, parametersBindingList.Values.ToArray());
			}
		}
		
#if UNITY_EDITOR

		protected override void OnValidate()
		{
			base.OnValidate();
			SanitizeTypes();
		}

		private void SanitizeTypes()
		{
			bool didTypeChange = false;

			if (expectedType?.Type == null)
			{
				expectedType = new SerializableType(InjectionType);
				didTypeChange = true;
			}

			if (didTypeChange)
			{
				BindingInfoTrackerProxy.RefreshBindingInfo();
			}
		}
#endif
	}
}
