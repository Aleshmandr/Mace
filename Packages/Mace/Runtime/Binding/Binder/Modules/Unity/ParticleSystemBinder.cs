using UnityEngine;

namespace Mace
{
    public class ParticleSystemBinder : ComponentBinder
    {
        [SerializeField] private BindingInfo play = BindingInfo.Variable<bool>();
        [SerializeField] private ParticleSystem target;

        protected override void Awake()
        {
            base.Awake();
            RegisterVariable<bool>(play).OnChanged(HandlePlayChange);
        }
        
        private void HandlePlayChange(bool value)
        {
            if (target != null)
            {
                return;
            }

            if (value)
            {
                target.Play(true);
            }
            else
            {
                target.Stop(true);
            }
        }
    }
}