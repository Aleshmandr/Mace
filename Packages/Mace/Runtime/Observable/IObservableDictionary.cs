using System.Collections.Generic;

namespace Mace
{
	public interface IObservableDictionary<TKey, TValue> : IReadOnlyObservableDictionary<TKey, TValue>, IDictionary<TKey, TValue>
	{

	}
}
