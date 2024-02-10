﻿using UnityEditor;
using UnityEngine;

namespace Mace.Editor
{
	[CustomPropertyDrawer(typeof(ConstantBindingInfo), true)]
	public class ConstantBindingInfoDrawer : BindingInfoDrawer
	{
		private static readonly string UseConstantName = "useConstant";
		private static readonly string ConstantName = "constant";

		protected override void DrawContent(Rect position, SerializedProperty property)
		{
			EditorGUI.BeginDisabledGroup(Application.isPlaying);

			position = DrawConstantToggle(position, property);

			SerializedProperty useConstantProp = property.FindPropertyRelative(UseConstantName);

			if (useConstantProp.boolValue)
			{
				EditorGUI.PropertyField(position, property.FindPropertyRelative(ConstantName), GUIContent.none);
			}
			else
			{
				DrawBindingInfo(position, property);
			}

			EditorGUI.EndDisabledGroup();
		}

		private Rect DrawConstantToggle(Rect position, SerializedProperty property)
		{
			float toggleWidth = EditorGUIUtility.singleLineHeight + 3;
			Rect toggleRect = new Rect(position.min, new Vector2(toggleWidth, position.height));

			SerializedProperty useConstantProp = property.FindPropertyRelative(UseConstantName);
			string buttonLabel = useConstantProp.boolValue ? "C" : "V";

			if (GUI.Button(toggleRect, buttonLabel))
			{
				useConstantProp.boolValue = !useConstantProp.boolValue;
			}

			float resultX = toggleRect.xMax + EditorGUIUtility.standardVerticalSpacing;
			float resultY = position.y;
			float resultWidth = position.width - toggleWidth - EditorGUIUtility.standardVerticalSpacing;
			float resultHeight = position.height;
			return new Rect(resultX, resultY, resultWidth, resultHeight);
		}
	}
}
