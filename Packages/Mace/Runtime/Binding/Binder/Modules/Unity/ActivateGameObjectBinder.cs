using System.Collections.Generic;
using UnityEngine;

namespace Mace
{
	public class ActivateGameObjectBinder : ComponentBinder
	{
		[SerializeField] private BindingInfo setActive = BindingInfo.Variable<bool>();
		[SerializeField] private List<ActivateGameObjectData> targets;

		protected override void Awake()
		{
			base.Awake();

			RegisterVariable<bool>(setActive).OnChanged(Refresh);
		}

		private void Refresh(bool value)
		{
			foreach (var current in targets)
			{
				if (current.Target == null)
				{
					continue;
				}

				current.Target.SetActive(current.Invert ? !value : value);
			}
		}
	}
}
