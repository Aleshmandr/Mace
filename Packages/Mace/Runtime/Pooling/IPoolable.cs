namespace Mace.Pooling
{
	public interface IPoolable
	{
		void OnSpawn();
		void OnRecycle();
	}
}
