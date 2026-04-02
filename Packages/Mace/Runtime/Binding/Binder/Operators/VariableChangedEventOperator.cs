using System;
using UnityEngine;

namespace Mace
{
    public class VariableChangedEventOperator : Operator
    {
        [SerializeField] private BindingInfo variable = BindingInfo.Variable<object>();
        private ObservableEvent exposedEvent;
        private int activationFrame;

        protected override void Awake()
        {
            base.Awake();
            exposedEvent = new ObservableEvent();
            RegisterVariable<object>(variable).OnChanged(HandleVariableChange);
            ViewModel = new EventViewModel(exposedEvent);
        }

        protected override void OnEnable()
        {
            activationFrame = Time.frameCount;
            base.OnEnable();
        }

        private void HandleVariableChange(object newValue)
        {
            if (activationFrame >= Time.frameCount)
            {
                return;
            }
            
            exposedEvent?.Raise();
        }

        protected override Type GetInjectionType()
        {
            return typeof(EventViewModel);
        }
    }
}