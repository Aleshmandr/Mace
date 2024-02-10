using UnityEngine;

namespace Mace
{
	public class ComponentBinder : MonoBehaviour
	{
		private BindingTracker bindingTracker;

		protected virtual void Awake()
		{
			bindingTracker = new BindingTracker(this);
		}

		protected virtual void OnEnable()
		{
			Bind();
		}

		protected virtual void OnDisable()
		{
			Unbind();
		}

		protected void Bind()
		{
			bindingTracker.Bind();
		}

		protected void Unbind()
		{
			bindingTracker.Unbind();
		}

		protected VariableBindingSubscriber<T> RegisterVariable<T>(BindingInfo bindingInfo)
		{
			return bindingTracker.RegisterVariable<T>(bindingInfo);
		}

		protected CollectionBindingSubscriber<T> RegisterCollection<T>(BindingInfo bindingInfo)
		{
			return bindingTracker.RegisterCollection<T>(bindingInfo);
		}

		protected EventBindingSubscriber RegisterEvent(BindingInfo bindingInfo)
		{
			return bindingTracker.RegisterEvent(bindingInfo);
		}

		protected EventBindingSubscriber<T> RegisterEvent<T>(BindingInfo bindingInfo)
		{
			return bindingTracker.RegisterEvent<T>(bindingInfo);
		}

		protected CommandBindingSubscriber RegisterCommand(BindingInfo bindingInfo)
		{
			return bindingTracker.RegisterCommand(bindingInfo);
		}

		protected CommandBindingSubscriber<T> RegisterCommand<T>(BindingInfo bindingInfo)
		{
			return bindingTracker.RegisterCommand<T>(bindingInfo);
		}
	}
}
