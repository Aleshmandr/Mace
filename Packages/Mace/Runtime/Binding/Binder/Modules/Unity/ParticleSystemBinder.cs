using Mace.Utils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
            if (value)
            {
                particles.Play(true);
            }
            else
            {
                particles.Stop(true);
            }
        }
        
#if UNITY_EDITOR
        [MenuItem("CONTEXT/ParticleSystem/Add Binder")]
        private static void AddBinder(MenuCommand command)
        {
            ParticleSystem context = (ParticleSystem) command.context;
            context.GetOrAddComponent<ParticleSystemBinder>();
        }
#endif
    }
}