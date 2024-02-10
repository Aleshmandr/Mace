﻿using UnityEngine;

namespace Mace.Utils
{
	public static class ComponentExtensions
	{
		public static T GetOrAddComponent<T>(this Component component) where T : Component
		{
			return component.gameObject.GetOrAddComponent<T>();
		}
	}
}
