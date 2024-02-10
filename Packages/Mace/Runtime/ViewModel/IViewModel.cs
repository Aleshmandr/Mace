namespace Mace
{
	public interface IViewModel
	{
		bool IsEnabled { get; }
		void Enable();
		void Disable();
	}
}
