﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Mace.Utils;
using UnityEngine;

namespace Mace.Utils
{
	public static class BindingUtils
	{
		private class BindingTypeChecker
		{
			private static readonly Type GenericVariableType = typeof(IReadOnlyObservableVariable<>);
			private static readonly Type GenericCollectionType = typeof(IReadOnlyObservableCollection<>);
			private static readonly Type GenericCommandType = typeof(IObservableCommand<>);
			private static readonly Type CommandType = typeof(IObservableCommand);
			private static readonly Type GenericEventType = typeof(IObservableEvent<>);
			private static readonly Type EventType = typeof(IObservableEvent);

			public Type TargetType
			{
				get => targetType;
				set
				{
					genericTypeToCheck = null;
					typeToCheck = null;
					Type genericType = value.GetGenericTypeTowardsRoot();

					if (genericType != null)
					{
						Type genericTypeDefinition = genericType.GetGenericTypeDefinition();

						if (genericType.GenericTypeArguments.Length > 0
						    && (genericTypeDefinition.ImplementsOrDerives(GenericVariableType)
						        || genericTypeDefinition.ImplementsOrDerives(GenericCollectionType)
						        || genericTypeDefinition.ImplementsOrDerives(GenericCommandType)
						        || genericTypeDefinition.ImplementsOrDerives(GenericEventType)))
						{
							genericTypeToCheck = genericTypeDefinition;
							typeToCheck =  genericType.GenericTypeArguments[0];
						}
					}
					else if (CommandType.IsAssignableFrom(value))
					{
						typeToCheck = CommandType;
					}
					else if (EventType.IsAssignableFrom(value))
					{
						typeToCheck = EventType;
					}

					targetType = value;
				}
			}

			public Type LastObservableType { get; private set; }
			public Type LastGenericArgument { get; private set; }

			private Type targetType;
			private Type genericTypeToCheck;
			private Type typeToCheck;

			public bool CanBeBound(Type type)
			{
				bool result = false;
				LastObservableType = null;
				LastGenericArgument = null;

				if (typeToCheck != null)
				{
					if (genericTypeToCheck != null)
					{
						Type genericType = type.GetGenericTypeTowardsRoot();

						if (genericType != null && genericType.GenericTypeArguments.Length > 0)
						{
							Type genericTypeDefinition = type.GetGenericTypeDefinition();
							Type genericArgument = genericType.GenericTypeArguments[0];

							LastObservableType = genericTypeDefinition;
							LastGenericArgument = genericArgument;

							result = genericTypeDefinition.ImplementsOrDerives(genericTypeToCheck)
							         && typeToCheck.IsAssignableFrom(genericArgument);
						}
					}
					else
					{
						LastObservableType = typeToCheck;
						result = typeToCheck.IsAssignableFrom(type);
					}
				}

				return result;
			}

			public bool NeedsToBeBoxed(Type type)
			{
				Type genericType = type.GetGenericTypeTowardsRoot();

				return typeToCheck != null
				       && targetType.IsGenericType
				       && genericType != null
				       && genericType.GenericTypeArguments.Length > 0
				       && genericType.GenericTypeArguments[0].IsValueType
				       && genericType.GenericTypeArguments[0] != typeToCheck;
			}
		}

		private static readonly BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.Instance;
		private static readonly BindingTypeChecker Checker = new BindingTypeChecker();

		public static IEnumerable<BindingEntry> GetBindings(Transform context, Type targetType)
		{
			Checker.TargetType = targetType;

			foreach (var contextComponent in GetComponentsInParents<ViewModelComponent>(context, true))
			{
				Type contextType = contextComponent.ExpectedType;

				if (contextType != null)
				{
					foreach (PropertyInfo propertyInfo in contextType.GetProperties(PropertyBindingFlags))
					{
						if (Checker.CanBeBound(propertyInfo.PropertyType))
						{
							bool needsToBeBoxed = Checker.NeedsToBeBoxed(propertyInfo.PropertyType);

							yield return new BindingEntry(
								contextComponent,
								propertyInfo.Name,
								needsToBeBoxed,
								Checker.LastObservableType,
								Checker.LastGenericArgument);
						}
					}
				}
			}
		}

		public static IEnumerable<BindingEntry> GetAllBindings(Type contextType, ViewModelComponent viewModelComponent = null)
		{
			foreach (PropertyInfo propertyInfo in contextType.GetProperties(PropertyBindingFlags))
			{
				Checker.TargetType = propertyInfo.PropertyType;

				if (Checker.CanBeBound(propertyInfo.PropertyType))
				{
					bool needsToBeBoxed = Checker.NeedsToBeBoxed(propertyInfo.PropertyType);

					yield return new BindingEntry(
						viewModelComponent,
						propertyInfo.Name,
						needsToBeBoxed,
						Checker.LastObservableType,
						Checker.LastGenericArgument);
				}
			}
		}

		public static bool CanBeBound(Type actualType, Type targetType)
		{
			Checker.TargetType = targetType;
			return Checker.CanBeBound(actualType);
		}

		public static bool NeedsToBeBoxed(Type actualType, Type targetType)
		{
			Checker.TargetType = targetType;
			return Checker.NeedsToBeBoxed(actualType);
		}

		private static IEnumerable<T> GetComponentsInParents<T>(Component component, bool includeSelf) where T : Component
		{
			if (includeSelf)
			{
				foreach (T current in component.GetComponents<T>())
				{
					yield return current;
				}
			}

			if (component.transform.parent)
			{
				foreach (T current in GetComponentsInParents<T>(component.transform.parent, true))
				{
					yield return current;
				}
			}
		}
	}
}
