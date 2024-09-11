using UnityEngine;

namespace Mace
{
	public class GameObjectBinder : ComponentBinder
	{
		[SerializeField] private BindingInfo objectName = BindingInfo.Variable<object>();

		protected override void Awake()
		{
			base.Awake();
			
			RegisterVariable<object>(objectName).OnChanged(HandleNameChange);
		}

		private void HandleNameChange(object newValue)
		{
			gameObject.name = newValue != null ? newValue.ToString() : string.Empty;
		}
	}
}
