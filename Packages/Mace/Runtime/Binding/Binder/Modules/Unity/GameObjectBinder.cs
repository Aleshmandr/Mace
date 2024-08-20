using UnityEngine;

namespace Mace
{
	public class GameObjectBinder : ComponentBinder
	{
		[SerializeField] private BindingInfo objectName = BindingInfo.Variable<object>();
		[SerializeField] private BindingInfo isActive = BindingInfo.Variable<bool>();

		protected override void Awake()
		{
			base.Awake();
			
			RegisterVariable<object>(objectName).OnChanged(HandleNameChange);
			RegisterVariable<bool>(isActive).OnChanged(HandleActiveStateChange);
		}

		protected override void OnEnable()
		{
			// Do nothing. Bind() moved to Start()
		}

		protected override void OnDisable()
		{
			// Do nothing. Unbind() moved to OnDestroy()
		}

		protected void Start()
		{
			Bind();
		}

		protected void OnDestroy()
		{
			Unbind();
		}

		private void HandleNameChange(object newValue)
		{
			gameObject.name = newValue != null ? newValue.ToString() : string.Empty;
		}

		private void HandleActiveStateChange(bool newValue)
		{
			gameObject.SetActive(newValue);
		}
	}
}
