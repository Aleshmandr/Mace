using System;
using System.Collections.Generic;

namespace Mace
{
    public sealed class ReadOnlyReactiveProperty<T> : IReadOnlyObservableVariable<T>, IDisposable
    {
        private readonly Func<bool> hasValueProvider;
        private readonly Func<T> valueProvider;
        private readonly Action subscribeToSources;
        private readonly Action unsubscribeFromSources;
        private readonly EqualityComparer<T> equalityComparer;

        private ObservableVariableEventHandler<T> changed;
        private ObservableVariableClearEventHandler cleared;
        private bool isSubscribedToSources;
        private bool cachedHasValue;
        private T cachedValue;
        private bool isDisposed;

        public event ObservableVariableEventHandler<T> Changed
        {
            add
            {
                if (isDisposed)
                {
                    return;
                }

                changed += value;
                UpdateSourceSubscription();
            }
            remove
            {
                changed -= value;
                UpdateSourceSubscription();
            }
        }

        public event ObservableVariableClearEventHandler Cleared
        {
            add
            {
                if (isDisposed)
                {
                    return;
                }

                cleared += value;
                UpdateSourceSubscription();
            }
            remove
            {
                cleared -= value;
                UpdateSourceSubscription();
            }
        }

        public bool HasValue => !isDisposed && (isSubscribedToSources ? cachedHasValue : hasValueProvider());
        public T Value => HasValue ? isSubscribedToSources ? cachedValue : valueProvider() : default;

        public ReadOnlyReactiveProperty(Func<bool> hasValueProvider, Func<T> valueProvider, Action subscribeToSources,
            Action unsubscribeFromSources, EqualityComparer<T> equalityComparer = null)
        {
            this.hasValueProvider = hasValueProvider ?? throw new ArgumentNullException(nameof(hasValueProvider));
            this.valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
            this.subscribeToSources = subscribeToSources ?? throw new ArgumentNullException(nameof(subscribeToSources));
            this.unsubscribeFromSources = unsubscribeFromSources ?? throw new ArgumentNullException(nameof(unsubscribeFromSources));
            this.equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            StopListening();
            changed = null;
            cleared = null;
            cachedHasValue = false;
            cachedValue = default;
            isDisposed = true;
        }

        internal void Refresh()
        {
            if (isDisposed)
            {
                return;
            }

            if (!hasValueProvider())
            {
                if (cachedHasValue)
                {
                    cachedHasValue = false;
                    cachedValue = default;
                    cleared?.Invoke();
                }

                return;
            }

            T newValue = valueProvider();
            if (!cachedHasValue || !equalityComparer.Equals(cachedValue, newValue))
            {
                cachedHasValue = true;
                cachedValue = newValue;
                changed?.Invoke(newValue);
            }
        }

        private void UpdateSourceSubscription()
        {
            if (isDisposed)
            {
                return;
            }

            bool shouldListen = changed != null || cleared != null;
            if (shouldListen && !isSubscribedToSources)
            {
                StartListening();
            }
            else if (!shouldListen && isSubscribedToSources)
            {
                StopListening();
            }
        }

        private void StartListening()
        {
            CaptureCurrentState();
            isSubscribedToSources = true;
            subscribeToSources();
        }

        private void StopListening()
        {
            if (!isSubscribedToSources)
            {
                return;
            }

            unsubscribeFromSources();
            isSubscribedToSources = false;
            cachedHasValue = false;
            cachedValue = default;
        }

        private void CaptureCurrentState()
        {
            cachedHasValue = hasValueProvider();
            cachedValue = cachedHasValue ? valueProvider() : default;
        }
    }
}
