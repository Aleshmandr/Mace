using Mace.Utils;
using Mace.Utils.Singleton;

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
				}

				return pool;
			}
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
