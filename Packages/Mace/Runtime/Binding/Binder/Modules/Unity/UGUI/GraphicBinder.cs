﻿using Mace.Utils;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mace
{
	public class GraphicBinder : ComponentBinder
	{
		[SerializeField] private BindingInfo color = BindingInfo.Variable<Color>();
		[SerializeField] private BindingInfo material = BindingInfo.Variable<Material>();
		[SerializeField] private BindingInfo raycastTarget = BindingInfo.Variable<bool>();

		private Graphic graphic;

		protected override void Awake()
		{
			base.Awake();

			graphic = GetComponent<Graphic>();

			if (graphic == null)
			{
				Debug.LogError($"{nameof(GraphicBinder)} requires a {nameof(Graphic)} to work.", this);
				return;
			}
			
			RegisterVariable<Color>(color).OnChanged(OnColorChanged);
			RegisterVariable<Material>(material).OnChanged(OnMaterialChanged);
			RegisterVariable<bool>(raycastTarget).OnChanged(OnRaycastTargetChanged);
		}

#if UNITY_EDITOR
		[MenuItem("CONTEXT/Graphic/Add Binder")]
		private static void AddBinder(MenuCommand command)
		{
			Graphic context = (Graphic) command.context;
			context.GetOrAddComponent<GraphicBinder>();
		}
#endif

		private void OnColorChanged(Color newValue)
		{
			graphic.color = newValue;
		}

		private void OnMaterialChanged(Material newValue)
		{
			graphic.material = newValue;
		}

		private void OnRaycastTargetChanged(bool newValue)
		{
			graphic.raycastTarget = newValue;
		}
	}
}
