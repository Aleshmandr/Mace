using System;
using UnityEngine;

namespace Mace
{
    public class EventMergeOperator : Operator
    {
        [SerializeField] private BindingInfoList events = new BindingInfoList(typeof(IObservableEvent));

        private ObservableEvent exposedEvent;

        protected override void Awake()
        {
            base.Awake();

            foreach (var eventBindingInfo in events.BindingInfos)
            {
                RegisterEvent(eventBindingInfo).OnRaised(RaiseExposedEvent);
            }
            
            exposedEvent = new ObservableEvent();
            ViewModel = new EventViewModel(exposedEvent);
        }

        private void RaiseExposedEvent()
        {
            exposedEvent.Raise();
        }

        protected override Type GetInjectionType()
        {
            return typeof(EventViewModel);
        }
    }
}