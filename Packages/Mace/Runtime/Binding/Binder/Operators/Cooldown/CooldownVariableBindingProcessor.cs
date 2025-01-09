using UnityEngine;

namespace Mace
{
	public class CooldownVariableBindingProcessor<T> : VariableBindingProcessor<T, T>
	{
		private readonly float cooldown;
		private float lastChangeTime;
		
		public CooldownVariableBindingProcessor(BindingInfo bindingInfo, Component viewModel, float cooldown)
			: base(bindingInfo, viewModel)
		{
			this.cooldown = cooldown;
			lastChangeTime = -cooldown;
		}

		public override void Bind()
		{
			lastChangeTime = -cooldown;
			base.Bind();
		}

		protected override void OnBoundVariableChanged(T newValue)
		{
			if (Time.unscaledTime - lastChangeTime >= cooldown)
			{
				lastChangeTime = Time.unscaledTime;
				base.OnBoundVariableChanged(newValue);
			}
		}

		protected override T ProcessValue(T value)
		{
			return value;
		}
	}
}