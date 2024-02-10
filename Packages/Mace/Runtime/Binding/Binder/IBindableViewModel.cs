namespace Mace
{
	public interface IBindableViewModel<in T>
	{
		void Set(T value);
	}
}
