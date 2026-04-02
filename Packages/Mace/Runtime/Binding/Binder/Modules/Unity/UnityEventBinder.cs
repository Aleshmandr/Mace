using UnityEngine;
using UnityEngine.Events;

namespace Mace
{
    public class UnityEventBinder : ComponentBinder
    {
        [SerializeField] private BindingInfo eventRaise = BindingInfo.Event();
        [SerializeField] private UnityEvent onEventRaise;
        [SerializeField] private BindingInfo variable = BindingInfo.Variable<object>();
        [SerializeField] private UnityEvent onVariableChange;
        private int activationFrame;

        protected override void Awake()
        {
            base.Awake();
            RegisterEvent(eventRaise).OnRaised(HandlePlayEventRaise);
            RegisterVariable<object>(variable).OnChanged(HandleVariableChange);
        }

        protected override void OnEnable()
        {
            activationFrame = Time.frameCount;
            base.OnEnable();
        }

        private void HandleVariableChange(object value)
        {
            if (activationFrame >= Time.frameCount)
            {
                return;
            }

            onVariableChange?.Invoke();
        }

        private void HandlePlayEventRaise()
        {
            onEventRaise?.Invoke();
        }
    }
}