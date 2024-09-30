using Mace.Utils;
using Mace.Utils.Singleton;

namespace Mace.Pooling
{
	public class SingleObjectPool : PersistentMonoSingleton<SingleObjectPool>
	{
		public ObjectPool GlobalPool
		{
			get
			{
				if (!pool)
				{
					pool = this.GetOrAddComponent<ObjectPool>();
				}

				return pool;
			}

		}

		private ObjectPool pool;
	}
}
