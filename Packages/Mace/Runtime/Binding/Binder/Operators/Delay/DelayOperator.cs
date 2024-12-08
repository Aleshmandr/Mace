using UnityEngine;

namespace Mace
{
    public abstract class DelayOperator<T> : ProcessorOperator<T, T>
    {
        [SerializeField] private float delay;
        private IUpdatableBindingProcessor updatableBindingProcessor;

        protected override IBindingProcessor GetBindingProcessor(BindingType bindingType, BindingInfo fromBinding)
        {
            switch (bindingType)
            {
                case BindingType.Variable:
                    updatableBindingProcessor = new DelayVariableBindingProcessor<T>(fromBinding, this, delay);
                    break;
                case BindingType.Collection:
                    updatableBindingProcessor = new DelayCollectionBindingProcessor<T>(fromBinding, this, delay);
                    break;
                case BindingType.Command:
                    updatableBindingProcessor = new DelayCommandBindingProcessor<T>(fromBinding, this, delay);
                    break;
                case BindingType.Event:
                    updatableBindingProcessor = new DelayEventBindingProcessor<T>(fromBinding, this, delay);
                    break;
            }

            return updatableBindingProcessor;
        }

        private void Update()
        {
            updatableBindingProcessor?.Update();
        }
    }
}