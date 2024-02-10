using System;

namespace Mace
{
	public interface IViewModelInjector
	{
		Type InjectionType { get; }
		ViewModelComponent Target { get; }
	}
}
