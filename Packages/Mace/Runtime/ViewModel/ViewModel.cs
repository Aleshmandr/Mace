namespace Mace
{
    public abstract class ViewModel : IViewModel
    {
        private int activeRefCount;
        
        public bool IsEnabled { get; private set; }

        public void Enable()
        {
            activeRefCount++;
            
            if (IsEnabled)
            {
                return;
            }

            IsEnabled = true;
            OnEnable();
        }

        public void Disable()
        {
            if (!IsEnabled)
            {
                return;
            }
            
            activeRefCount--;

            if (activeRefCount > 0)
            {
                return;
            }

            IsEnabled = false;
            OnDisable();
        }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }
    }
}