using System;
using System.Collections.Generic;
using Mace.Utils;
using UnityEngine;

namespace Mace
{
	public static class ViewModelComponentTree
	{
		private static readonly Dictionary<Transform, ViewModelComponentTreeNode> NodesByTransform;
		private static readonly Dictionary<ViewModelComponent, ViewModelComponentTreeNode> NodesByComponent;

		static ViewModelComponentTree()
		{
			NodesByTransform = new Dictionary<Transform, ViewModelComponentTreeNode>();
			NodesByComponent = new Dictionary<ViewModelComponent, ViewModelComponentTreeNode>();
		}

		public static void Register(ViewModelComponent component)
		{
			if (NodesByComponent.ContainsKey(component) == false)
			{
				AddComponent(component);
			}
		}

		public static void Unregister(ViewModelComponent component)
		{
			if (NodesByComponent.TryGetValue(component, out ViewModelComponentTreeNode node))
			{
				node.Components.Remove(component.Id);
				NodesByComponent.Remove(component);

				if (node.Components.Count == 0)
				{
					foreach (ViewModelComponentTreeNode current in node.Children)
					{
						current.SetParentNode(node.ParentNode);
					}

					NodesByTransform.Remove(component.transform);
				}
			}
		}

		public static void Move(ViewModelComponent component)
		{
			if (NodesByComponent.TryGetValue(component, out ViewModelComponentTreeNode node))
			{
				Transform transform = component.transform;

				if (node.ParentObject != transform.parent)
				{
					ReparentNode(transform, node);
				}
			}
			else
			{
				Register(component);
			}
		}

		public static ViewModelComponent FindBindableComponent(string path, Type targetType, Transform context)
		{
			ViewModelComponent result = null;
			Transform currentTransform = context;
			ViewModelComponentTreeNode currentNode;

			while (result == null && (currentNode = GetNextNodeTowardsRoot(currentTransform)) != null)
			{
				using (var components = currentNode.Components.Values.GetEnumerator())
				{
					while (result == null && components.MoveNext())
					{
						if (CanBeBound(components.Current, path, targetType))
						{
							result = components.Current?.Component;
						}
					}
				}

				currentTransform = currentNode.Transform.parent;
			}

			return result;
		}

		public static object Bind(string path, ViewModelComponent component)
		{
			Register(component);
			ViewModelComponentTreeNode node = NodesByComponent[component];
			return node.Components[component.Id].GetProperty(path);
		}

		private static void AddComponent(ViewModelComponent component)
		{
			Transform transform = component.transform;

			if (NodesByTransform.TryGetValue(transform, out var node) == false)
			{
				node = new ViewModelComponentTreeNode(transform);
				ReparentNode(transform, node);
				NodesByTransform[transform] = node;
			}

			node.AddComponent(component);
			NodesByComponent.Add(component, node);
		}

		private static void ReparentNode(Transform transform, ViewModelComponentTreeNode node)
		{
			ViewModelComponentTreeNode parentNode = FindParentNode(transform);
			node.SetParentNode(parentNode);
			node.ParentObject = transform.parent;
		}

		private static ViewModelComponentTreeNode FindParentNode(Transform transform)
		{
			Transform parentTransform = transform.parent;
			ViewModelComponentTreeNode parentNode = null;

			while (parentTransform && NodesByTransform.TryGetValue(parentTransform, out parentNode) == false)
			{
				parentTransform = parentTransform.parent;
			}

			return parentNode;
		}

		private static bool CanBeBound(ViewModelComponentInfo componentInfo, string propertyName, Type targetType)
		{
			object property = componentInfo.GetProperty(propertyName);
			return property != null && BindingUtils.CanBeBound(property.GetType(), targetType);
		}

		private static ViewModelComponentTreeNode GetNextNodeTowardsRoot(Transform context)
		{
			ViewModelComponentTreeNode result = null;
			Transform currentTransform = context;

			while (currentTransform != null && !NodesByTransform.TryGetValue(currentTransform, out result))
			{
				currentTransform = currentTransform.parent;
			}

			return result;
		}
	}
}
