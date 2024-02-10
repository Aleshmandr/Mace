using UnityEngine;

namespace Mace
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemBinder : ComponentBinder
    {
        [SerializeField] private BindingInfo play = BindingInfo.Variable<bool>();
        private ParticleSystem particles;

        protected override void Awake()
        {
            base.Awake();
            particles = GetComponent<ParticleSystem>();
            RegisterVariable<bool>(play).OnChanged(HandlePlayChange);
        }
        
        private void HandlePlayChange(bool value)
        {
            if (particles == null)
            {
                return;
            }

            if (value)
            {
                particles.Play(true);
            }
            else
            {
                particles.Stop(true);
            }
        }
    }
}