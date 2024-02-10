using System.Collections.Generic;

namespace Mace
{
	public interface IObservableCollection<T> : IList<T>, IReadOnlyObservableCollection<T>
	{
		new int Count { get; }
		new T this[int index] { get; set; }

		void Move(int oldIndex, int newIndex);
	}
}
