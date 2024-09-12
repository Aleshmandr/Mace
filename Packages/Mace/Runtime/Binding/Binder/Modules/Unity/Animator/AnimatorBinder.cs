using System;
using System.Collections.Generic;
using Mace.Utils;
using UnityEditor;
using UnityEngine;

namespace Mace
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorBinder : ComponentBinder
    {
        [SerializeField] private AnimatorBoolParameterBindingInfo[] boolParameters;
        [SerializeField] private AnimatorIntParameterBindingInfo[] intParameters;
        [SerializeField] private AnimatorFloatParameterBindingInfo[] floatParameters;
        [SerializeField] private AnimatorBoolParameterBindingInfo[] triggerParameters;
        [SerializeField] private AnimatorEventBindingInfo[] setTriggerEvents;
        private Animator animator;

        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
            RegisterParameters(boolParameters, (parameterId, value) => animator.SetBool(parameterId, value));
            RegisterParameters(intParameters, (parameterId, value) => animator.SetInteger(parameterId, value));
            RegisterParameters(floatParameters, (parameterId, value) => animator.SetFloat(parameterId, value));
            RegisterParameters(triggerParameters, (parameterId, value) =>
            {
                if (value)
                {
                    animator.SetTrigger(parameterId);
                }
                else
                {
                    animator.ResetTrigger(parameterId);
                }
            });
            RegisterEvents(setTriggerEvents, parameterId => animator.SetTrigger(parameterId));
        }

        private void RegisterEvents(AnimatorEventBindingInfo[] events, Action<int> setAnimatorParameter)
        {
            if (events == null)
            {
                return;
            }

            foreach (var parameter in events)
            {
                RegisterEvent(parameter.Binding).OnRaised(() => setAnimatorParameter.Invoke(parameter.Id));
            }
        }
        
        private void RegisterEvents<T>(AnimatorEventBindingInfo[] events, Action<int, T> setAnimatorParameter)
        {
            if (events == null)
            {
                return;
            }

            foreach (var parameter in events)
            {
                RegisterEvent<T>(parameter.Binding).OnRaised(value => setAnimatorParameter(parameter.Id, value));
            }
        }
        
        private void RegisterParameters<T>(IEnumerable<AnimatorParameterBindingInfo<T>> parameters, Action<int, T> setAnimatorParameter)
        {
            if (parameters == null)
            {
                return;
            }

            foreach (var parameter in parameters)
            {
                RegisterParameter(parameter).OnChanged(value => setAnimatorParameter(parameter.Id, value));
            }
        }

        private VariableBindingSubscriber<T> RegisterParameter<T>(AnimatorParameterBindingInfo<T> parameter)
        {
            return RegisterVariable<T>(parameter.Binding);
        }


#if UNITY_EDITOR
        [MenuItem("CONTEXT/Animator/Add Binder")]
        private static void AddBinder(MenuCommand command)
        {
            Animator animator = (Animator)command.context;
            animator.GetOrAddComponent<AnimatorBinder>();
        }

        private void OnValidate()
        {
            ValidateParameters(boolParameters);
            ValidateParameters(intParameters);
            ValidateParameters(floatParameters);
            ValidateParameters(triggerParameters);
            ValidateEvents(setTriggerEvents);
        }

        private void ValidateEvents(IEnumerable<AnimatorEventBindingInfo> events)
        {
            if (events == null)
            {
                return;
            }
            
            foreach (var bindingInfo in events)
            {
                bindingInfo.ValidateBinding();
            }
        }
        
        private void ValidateParameters<T>(IEnumerable<AnimatorParameterBindingInfo<T>> parameters)
        {
            if (parameters == null)
            {
                return;
            }
            
            foreach (var bindingInfo in parameters)
            {
                bindingInfo.ValidateBinding();
            }
        }
#endif
    }
}