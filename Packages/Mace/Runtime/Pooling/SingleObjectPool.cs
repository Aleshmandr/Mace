using Mace.Utils;
using Mace.Utils.Singleton;
using UnityEngine;

namespace Mace.Pooling
{
	internal class SingleObjectPool : PersistentMonoSingleton<SingleObjectPool>
	{
		private GlobalPool pool;
		public static bool IsDisposed;
		
		public GlobalPool GlobalPool
		{
			get
			{
				if (!pool)
				{
					pool = this.GetOrAddComponent<GlobalPool>();
					IsDisposed = false;
				}

				return pool;
			}
		}
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeOnLoad()
        {
        	IsDisposed = false;
        }

		private void OnDestroy()
		{
			if (Instance == this)
			{
				IsDisposed = true;
			}
		}
	}
}
