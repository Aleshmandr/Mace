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
            if (trailRenderer == null)
            {
                return;
            }

            trailRenderer.time = value;
        }

        private void HandleEmitChange(bool value)
        {
            if (trailRenderer == null)
            {
                return;
            }

            trailRenderer.emitting = value;
        }
    }
}