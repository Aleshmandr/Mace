﻿using UnityEngine;

namespace Mace
{
    public class FloatDampingOperator : ProcessorOperator<float, float>
    {
        [SerializeField] private float damping = 1f;
        [SerializeField] private bool useUnscaledTime;
        private FloatVariableDampingProcessor dampingProcessor;
        
        protected override IBindingProcessor GetBindingProcessor(BindingType bindingType, BindingInfo fromBinding)
        {
            dampingProcessor = new FloatVariableDampingProcessor(fromBinding, this, damping, useUnscaledTime);
            return dampingProcessor;
        }
        
        private void Update()
        {
            dampingProcessor?.Update();
        }
    }
}