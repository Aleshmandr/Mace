using System;

namespace Mace
{
    public static class ReactivePropertyExtensions
    {
        public static ReadOnlyReactiveProperty<TResult> Select<TSource, TResult>(this IReadOnlyObservableVariable<TSource> source,
            Func<TSource, TResult> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            ReadOnlyReactiveProperty<TResult> property = null;
            ObservableVariableEventHandler<TSource> sourceChanged = _ => property.Refresh();
            ObservableVariableClearEventHandler sourceCleared = () => property.Refresh();

            property = new ReadOnlyReactiveProperty<TResult>(
                () => source.HasValue,
                () => selector(source.Value),
                () =>
                {
                    source.Changed += sourceChanged;
                    source.Cleared += sourceCleared;
                },
                () =>
                {
                    source.Changed -= sourceChanged;
                    source.Cleared -= sourceCleared;
                });

            return property;
        }

        public static ReadOnlyReactiveProperty<TResult> CombineLatest<TSource1, TSource2, TResult>(
            this IReadOnlyObservableVariable<TSource1> source1, IReadOnlyObservableVariable<TSource2> source2,
            Func<TSource1, TSource2, TResult> selector)
        {
            if (source1 == null)
            {
                throw new ArgumentNullException(nameof(source1));
            }

            if (source2 == null)
            {
                throw new ArgumentNullException(nameof(source2));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            ReadOnlyReactiveProperty<TResult> property = null;
            ObservableVariableEventHandler<TSource1> source1Changed = _ => property.Refresh();
            ObservableVariableClearEventHandler source1Cleared = () => property.Refresh();
            ObservableVariableEventHandler<TSource2> source2Changed = _ => property.Refresh();
            ObservableVariableClearEventHandler source2Cleared = () => property.Refresh();

            property = new ReadOnlyReactiveProperty<TResult>(
                () => source1.HasValue && source2.HasValue,
                () => selector(source1.Value, source2.Value),
                () =>
                {
                    source1.Changed += source1Changed;
                    source1.Cleared += source1Cleared;
                    source2.Changed += source2Changed;
                    source2.Cleared += source2Cleared;
                },
                () =>
                {
                    source1.Changed -= source1Changed;
                    source1.Cleared -= source1Cleared;
                    source2.Changed -= source2Changed;
                    source2.Cleared -= source2Cleared;
                });

            return property;
        }

        public static ReadOnlyReactiveProperty<TResult> CombineLatest<TSource1, TSource2, TSource3, TResult>(
            this IReadOnlyObservableVariable<TSource1> source1, IReadOnlyObservableVariable<TSource2> source2,
            IReadOnlyObservableVariable<TSource3> source3, Func<TSource1, TSource2, TSource3, TResult> selector)
        {
            if (source1 == null)
            {
                throw new ArgumentNullException(nameof(source1));
            }

            if (source2 == null)
            {
                throw new ArgumentNullException(nameof(source2));
            }

            if (source3 == null)
            {
                throw new ArgumentNullException(nameof(source3));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            ReadOnlyReactiveProperty<TResult> property = null;
            ObservableVariableEventHandler<TSource1> source1Changed = _ => property.Refresh();
            ObservableVariableClearEventHandler source1Cleared = () => property.Refresh();
            ObservableVariableEventHandler<TSource2> source2Changed = _ => property.Refresh();
            ObservableVariableClearEventHandler source2Cleared = () => property.Refresh();
            ObservableVariableEventHandler<TSource3> source3Changed = _ => property.Refresh();
            ObservableVariableClearEventHandler source3Cleared = () => property.Refresh();

            property = new ReadOnlyReactiveProperty<TResult>(
                () => source1.HasValue && source2.HasValue && source3.HasValue,
                () => selector(source1.Value, source2.Value, source3.Value),
                () =>
                {
                    source1.Changed += source1Changed;
                    source1.Cleared += source1Cleared;
                    source2.Changed += source2Changed;
                    source2.Cleared += source2Cleared;
                    source3.Changed += source3Changed;
                    source3.Cleared += source3Cleared;
                },
                () =>
                {
                    source1.Changed -= source1Changed;
                    source1.Cleared -= source1Cleared;
                    source2.Changed -= source2Changed;
                    source2.Cleared -= source2Cleared;
                    source3.Changed -= source3Changed;
                    source3.Cleared -= source3Cleared;
                });

            return property;
        }

        public static ReadOnlyReactiveProperty<bool> Not(this IReadOnlyObservableVariable<bool> source)
        {
            return source.Select(value => !value);
        }

        public static ReadOnlyReactiveProperty<bool> And(this IReadOnlyObservableVariable<bool> source1,
            IReadOnlyObservableVariable<bool> source2)
        {
            return source1.CombineLatest(source2, (value1, value2) => value1 && value2);
        }

        public static ReadOnlyReactiveProperty<bool> Or(this IReadOnlyObservableVariable<bool> source1,
            IReadOnlyObservableVariable<bool> source2)
        {
            return source1.CombineLatest(source2, (value1, value2) => value1 || value2);
        }
    }
}
