using System;
using Mace.Utils.Singleton;

namespace Mace.Utils
{
	public class LifecycleUtils : PersistentMonoSingleton<LifecycleUtils>
	{
		public static event Action OnUpdate
		{
			add => Instance.onUpdate += value;
			remove => Instance.onUpdate -= value;
		}

		public static event Action OnLateUpdate
		{
			add => Instance.onLateUpdate += value;
			remove => Instance.onLateUpdate -= value;
		}

		private event Action onUpdate;
		private event Action onLateUpdate;

		private void Update()
		{
			onUpdate?.Invoke();
		}

		private void LateUpdate()
		{
			onLateUpdate?.Invoke();
		}

		private void OnDestroy()
		{
			onUpdate = null;
			onLateUpdate = null;
		}
	}
}
