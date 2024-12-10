namespace Mace
{
    public class ParentViewModelReference : ViewModelComponent
    {
        private ViewModelComponent parentViewModel;

        protected override void OnEnable()
        {
            base.OnEnable();
            parentViewModel = GetComponentInParent<ViewModelComponent>();
            if (parentViewModel != null)
            {
                ViewModel = parentViewModel.ViewModel;
                parentViewModel.ViewModelChanged += HandleParentViewModelChange;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (parentViewModel != null)
            {
                parentViewModel.ViewModelChanged -= HandleParentViewModelChange;
            }

            ViewModel = null;
        }

        private void HandleParentViewModelChange(IViewModelProvider<IViewModel> source, IViewModel lastViewModel, IViewModel newViewModel)
        {
            ViewModel = newViewModel;
        }

        protected override void OnViewModelDisable()
        {
            // Do nothing
        }

        protected override void OnViewModelEnable()
        {
            // Do nothing
        }
    }
}