using Mace.Utils;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mace
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemBinder : ComponentBinder
    {
        [SerializeField] private BindingInfo isPlaying = BindingInfo.Variable<bool>();
        [SerializeField] private BindingInfo play = BindingInfo.Event();
        [SerializeField] private BindingInfo color = BindingInfo.Variable<Color>();
        private ParticleSystem particles;

        protected override void Awake()
        {
            base.Awake();
            particles = GetComponent<ParticleSystem>();
            RegisterVariable<bool>(isPlaying).OnChanged(HandlePlayChange);
            RegisterEvent(play).OnRaised(HandlePlayEventRaise);
            RegisterVariable<Color>(color).OnChanged(HandleColorChange);
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
        
        private void HandlePlayEventRaise()
        {
            particles.Play(true);
        }

        private void HandleColorChange(Color color)
        {
            var main = particles.main;
            main.startColor = color;
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