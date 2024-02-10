namespace Mace
{
	public interface IBindingProcessor
	{
		IViewModel ViewModel { get; }

		void Bind();
		void Unbind();
	}
}