using System;
using UnityEngine;

namespace Mace
{
	public class GameObjectBinder : ComponentBinder
	{
		[SerializeField] private BindingInfo objectName = BindingInfo.Variable<object>();
		[SerializeField] private BindingInfo isActive = BindingInfo.Variable<bool>();
		[SerializeField] private BindingInfo onActivityStateChanged = BindingInfo.Command<bool>();
		private event Action<bool> activityChanged;
		
		protected override void Awake()
		{
			base.Awake();
			
			RegisterVariable<object>(objectName).OnChanged(HandleNameChange);
			RegisterVariable<bool>(isActive).OnChanged(HandleActiveStateChange);
			RegisterCommand<bool>(onActivityStateChanged).AddExecuteTrigger(AddActivityTrigger);
		}

		private void AddActivityTrigger(Action<bool> action)
		{
			activityChanged += action;
		}

		protected override void OnEnable()
		{
			activityChanged?.Invoke(true);
		}

		protected override void OnDisable()
		{
			activityChanged?.Invoke(false);
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
