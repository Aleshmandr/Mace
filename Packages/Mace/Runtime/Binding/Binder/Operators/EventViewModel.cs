namespace Mace
{
    public class EventViewModel : ViewModel
    {
        public IObservableEvent Value { get; }

        public EventViewModel(IObservableEvent value)
        {
            Value = value;
        }
    }
}