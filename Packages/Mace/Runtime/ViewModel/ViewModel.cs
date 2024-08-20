namespace Mace
{
    public abstract class ViewModel : IViewModel
    {
        public bool IsEnabled { get; private set; }

        public void Enable()
        {
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

            IsEnabled = false;
            OnDisable();
        }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }
    }
}
