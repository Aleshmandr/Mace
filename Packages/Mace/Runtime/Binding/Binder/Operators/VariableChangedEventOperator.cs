using System;
using UnityEngine;

namespace Mace
{
    public class VariableChangedEventOperator : Operator
    {
        [SerializeField] private BindingInfo variable = BindingInfo.Variable<object>();
        private ObservableEvent exposedEvent;

        protected override void Awake()
        {
            base.Awake();
            RegisterVariable<object>(variable).OnChanged(HandleVariableChange);
            exposedEvent = new ObservableEvent();
            ViewModel = new EventViewModel(exposedEvent);
        }
        
        private void HandleVariableChange(object newValue)
        {
            exposedEvent.Raise();
        }

        protected override Type GetInjectionType()
        {
            return typeof(EventViewModel);
        }
    }
}