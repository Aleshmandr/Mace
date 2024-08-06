#if TEXTMESHPRO_PRESENTS
using Mace.Utils;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mace
{
	[RequireComponent(typeof(TextMeshPro))]
	public class TextMeshProBinder : ComponentBinder
	{
		[SerializeField] private BindingInfo text = BindingInfo.Variable<object>();
		[SerializeField] private BindingInfo color = BindingInfo.Variable<Color>();
		[SerializeField] private BindingInfo fontMaterial = BindingInfo.Variable<Material>();

		private TextMeshPro textComponent;

		protected override void Awake()
		{
			base.Awake();

			textComponent = GetComponent<TextMeshPro>();

			RegisterVariable<object>(text).OnChanged(OnTextChanged);
			RegisterVariable<Color>(color).OnChanged(OnColorChanged);
			RegisterVariable<Material>(fontMaterial).OnChanged(OnFontMaterialChanged);
		}

#if UNITY_EDITOR
		[MenuItem("CONTEXT/TextMeshPro/Add Binder")]
		private static void AddBinder(MenuCommand command)
		{
			TextMeshPro context = (TextMeshPro) command.context;
			context.GetOrAddComponent<TextMeshProBinder>();
		}
#endif
		private void OnTextChanged(object newValue)
		{
			textComponent.text = newValue != null ? newValue.ToString() : string.Empty;
		}
		
		private void OnColorChanged(Color newValue)
		{
			textComponent.color = newValue;
		}
		
		private void OnFontMaterialChanged(Material newValue)
		{
			textComponent.fontSharedMaterial = newValue;
		}
	}
}
#endif