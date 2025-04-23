using System.Collections.Generic;
using UnityEngine;

namespace Mace
{
	public class EnableBehaviourBinder : ComponentBinder
	{
		[SerializeField] private BindingInfo isEnabled = BindingInfo.Variable<bool>();
		[SerializeField] private List<EnableBehaviourData> targets;

		protected override void Awake()
		{
			base.Awake();
			RegisterVariable<bool>(isEnabled).OnChanged(Refresh);
		}

		private void Refresh(bool value)
		{
			foreach (var current in targets)
			{
				if (current.Target == null)
				{
					continue;
				}
				
				current.Target.enabled = current.Invert ? !value : value;
			}
		}
	}
}
