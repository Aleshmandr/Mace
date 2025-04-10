using UnityEngine;

namespace Mace
{
    public class IntDampingOperator : ProcessorOperator<int, int>
    {
        [SerializeField] private float damping = 1f;
        [SerializeField] private bool useUnscaledTime;
        private IntVariableDampingProcessor dampingProcessor;

        protected override IBindingProcessor GetBindingProcessor(BindingType bindingType, BindingInfo fromBinding)
        {
            dampingProcessor = new IntVariableDampingProcessor(fromBinding, this, damping, useUnscaledTime);
            return dampingProcessor;
        }

        private void Update()
        {
            dampingProcessor?.Update();
        }
    }
}