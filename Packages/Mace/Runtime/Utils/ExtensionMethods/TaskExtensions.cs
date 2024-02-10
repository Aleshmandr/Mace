﻿using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Mace.Utils
{
	public static class TaskExtensions
	{
		public static async void RunAndForget(this Task task, Action<Exception> onException = null)
		{
			try
			{
				await task;
			}
			catch (Exception ex)
			{
				if (onException != null)
				{
					onException(ex);
				}
				else
				{
					Debug.LogError(ex);
				}
			}
		}
	}
}
