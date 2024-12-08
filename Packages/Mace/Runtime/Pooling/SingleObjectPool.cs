using Mace.Utils;
using Mace.Utils.Singleton;

namespace Mace.Pooling
{
	public class SingleObjectPool : PersistentMonoSingleton<SingleObjectPool>
	{
		private ObjectPool pool;
		
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
	}
}
