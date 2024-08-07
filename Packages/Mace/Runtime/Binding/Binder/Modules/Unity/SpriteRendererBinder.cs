using Mace.Utils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mace
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRendererBinder : ComponentBinder
    {
        [SerializeField] private BindingInfo sprite = BindingInfo.Variable<Sprite>();
        [SerializeField] private BindingInfo flipX = BindingInfo.Variable<bool>();
        [SerializeField] private BindingInfo flipY = BindingInfo.Variable<bool>();
        [SerializeField] private BindingInfo material = BindingInfo.Variable<Material>();
        private SpriteRenderer spriteRenderer;

        protected override void Awake()
        {
            base.Awake();
            spriteRenderer = GetComponent<SpriteRenderer>();
            RegisterVariable<Sprite>(sprite).OnChanged(HandleSpriteChange);
            RegisterVariable<bool>(flipX).OnChanged(HandleFlipXChange);
            RegisterVariable<bool>(flipY).OnChanged(HandleFlipYChange);
            RegisterVariable<Material>(material).OnChanged(HandleMaterialChange);
        }

        private void HandleMaterialChange(Material value)
        {
            spriteRenderer.material = value;
        }

        private void HandleFlipXChange(bool value)
        {
            spriteRenderer.flipX = value;
        }

        private void HandleFlipYChange(bool value)
        {
            spriteRenderer.flipY = value;
        }

        private void HandleSpriteChange(Sprite value)
        {
            spriteRenderer.sprite = value;
        }

#if UNITY_EDITOR
        [MenuItem("CONTEXT/SpriteRenderer/Add Binder")]
        private static void AddBinder(MenuCommand command)
        {
            SpriteRenderer context = (SpriteRenderer)command.context;
            context.GetOrAddComponent<SpriteRendererBinder>();
        }
#endif
    }
}