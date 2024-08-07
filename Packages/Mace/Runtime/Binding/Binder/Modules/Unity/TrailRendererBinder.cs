using Mace.Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Mace
{
    [RequireComponent(typeof(TrailRenderer))]
    public class TrailRendererBinder : ComponentBinder
    {
        [SerializeField] private BindingInfo isEmitting = BindingInfo.Variable<bool>();
        [SerializeField] private BindingInfo time = BindingInfo.Variable<float>();
        private TrailRenderer trailRenderer;

        protected override void Awake()
        {
            base.Awake();
            trailRenderer = GetComponent<TrailRenderer>();
            RegisterVariable<bool>(isEmitting).OnChanged(HandleEmitChange);
            RegisterVariable<float>(time).OnChanged(HandleTimeChange);
        }

        private void HandleTimeChange(float value)
        {
            trailRenderer.time = value;
        }

        private void HandleEmitChange(bool value)
        {
            trailRenderer.emitting = value;
        }
        
#if UNITY_EDITOR
        [MenuItem("CONTEXT/TrailRenderer/Add Binder")]
        private static void AddBinder(MenuCommand command)
        {
            TrailRenderer context = (TrailRenderer) command.context;
            context.GetOrAddComponent<TrailRendererBinder>();
        }
#endif
    }
}