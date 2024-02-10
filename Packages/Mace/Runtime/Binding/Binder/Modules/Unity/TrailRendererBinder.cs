using UnityEngine;

namespace Mace
{
    public class TrailRendererBinder : ComponentBinder
    {
        [SerializeField] private BindingInfo isEmitting = BindingInfo.Variable<bool>();
        [SerializeField] private BindingInfo time = BindingInfo.Variable<float>();
        [SerializeField] private TrailRenderer target;

        protected override void Awake()
        {
            base.Awake();
            RegisterVariable<bool>(isEmitting).OnChanged(HandleEmitChange);
            RegisterVariable<float>(time).OnChanged(HandleTimeChange);
        }

        private void HandleTimeChange(float value)
        {
            if (target != null)
            {
                return;
            }

            target.time = value;
        }

        private void HandleEmitChange(bool value)
        {
            if (target != null)
            {
                return;
            }

            target.emitting = value;
        }
    }
}