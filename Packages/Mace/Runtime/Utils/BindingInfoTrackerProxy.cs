using System;

namespace Mace.Utils
{
	public class BindingInfoTrackerProxy
	{
		public static event Action RefreshBindingInfoRequested;

		public static void RefreshBindingInfo()
		{
			RefreshBindingInfoRequested?.Invoke();
		}
	}
}