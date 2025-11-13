using Mace;
using System;
using UnityEngine;
using Mace.Utils;
using System.Text;

namespace Mace
{
    public class FloatToSmartTimeFormatOperator : ViewModelComponent, IViewModelInjector
    {
        [SerializeField] private BindingInfo fromSeconds = BindingInfo.Variable<float>();
        [SerializeField] [Min(1)] private int valuesToShow = 3;
        [SerializeField] private bool trimLeadingZeros;
        [SerializeField] private ConstantBindingInfo<string> daysFormat = new();
        [SerializeField] private ConstantBindingInfo<string> hoursFormat = new();
        [SerializeField] private ConstantBindingInfo<string> minutesFormat = new();
        [SerializeField] private ConstantBindingInfo<string> secondsFormat = new();
        [SerializeField] private ConstantBindingInfo<string> millisecondsFormat = new();
        private VariableBinding<float> timerBinding;
        private VariableBinding<string> daysFormatBinding;
        private VariableBinding<string> hoursFormatBinding;
        private VariableBinding<string> minutesFormatBinding;
        private VariableBinding<string> secondsFormatBinding;
        private VariableBinding<string> millisecondsFormatBinding;
        private ObservableVariable<string> exposedProperty;
        private readonly StringBuilder stringBuilder = new(32);
        private readonly BufferValue[] buffer = new BufferValue[5];

        private struct BufferValue
        {
            public int Index;
            public int Value;
            public string Text;
        }

        public Type InjectionType => typeof(OperatorVariableViewModel<string>);
        public ViewModelComponent Target => this;

        protected override void Awake()
        {
            base.Awake();
            exposedProperty = new ObservableVariable<string>();
            ViewModel = new OperatorVariableViewModel<string>(exposedProperty);
            daysFormatBinding = new VariableBinding<string>(daysFormat, this);
            hoursFormatBinding = new VariableBinding<string>(hoursFormat, this);
            minutesFormatBinding = new VariableBinding<string>(minutesFormat, this);
            secondsFormatBinding = new VariableBinding<string>(secondsFormat, this);
            millisecondsFormatBinding = new VariableBinding<string>(millisecondsFormat, this);
            timerBinding = new VariableBinding<float>(fromSeconds, this);
        }

        protected virtual void OnEnable()
        {
            base.OnEnable();
            daysFormatBinding.Property.Changed += OnFormatChanged;
            hoursFormatBinding.Property.Changed += OnFormatChanged;
            minutesFormatBinding.Property.Changed += OnFormatChanged;
            secondsFormatBinding.Property.Changed += OnFormatChanged;
            millisecondsFormatBinding.Property.Changed += OnFormatChanged;
            timerBinding.Property.Changed += OnTimerChanged;
            daysFormatBinding.Bind();
            hoursFormatBinding.Bind();
            minutesFormatBinding.Bind();
            secondsFormatBinding.Bind();
            millisecondsFormatBinding.Bind();
            timerBinding.Bind();
        }

        protected virtual void OnDisable()
        {
            base.OnDisable();
            daysFormatBinding.Unbind();
            hoursFormatBinding.Unbind();
            minutesFormatBinding.Unbind();
            secondsFormatBinding.Unbind();
            millisecondsFormatBinding.Unbind();
            timerBinding.Unbind();
            daysFormatBinding.Property.Changed -= OnFormatChanged;
            hoursFormatBinding.Property.Changed -= OnFormatChanged;
            minutesFormatBinding.Property.Changed -= OnFormatChanged;
            secondsFormatBinding.Property.Changed -= OnFormatChanged;
            millisecondsFormatBinding.Property.Changed -= OnFormatChanged;
            timerBinding.Property.Changed -= OnTimerChanged;
            exposedProperty.Clear();
        }

        private void OnTimerChanged(float timer)
        {
            RefreshExposedValue();
        }

        private void OnFormatChanged(string newValue)
        {
            RefreshExposedValue();
        }

        private void RefreshExposedValue()
        {
            float secondsFloat = Mathf.Max(0f, timerBinding.Property.Value);
            TimeSpan timeSpan = TimeSpan.FromSeconds(secondsFloat);

            int days = timeSpan.Days;
            int hours = timeSpan.Hours;
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;
            int milliseconds = timeSpan.Milliseconds;

            bool hasFormatDays = HasFormat(0);
            bool hasFormatHours = HasFormat(1);
            bool hasFormatMinutes = HasFormat(2);
            bool hasFormatSeconds = HasFormat(3);
            bool hasFormatMilliseconds = HasFormat(4);

            int formattedCount = 0;
            if (hasFormatDays) formattedCount++;
            if (hasFormatHours) formattedCount++;
            if (hasFormatMinutes) formattedCount++;
            if (hasFormatSeconds) formattedCount++;
            if (hasFormatMilliseconds) formattedCount++;

            if (formattedCount == 0)
            {
                exposedProperty.Value = string.Empty;
                return;
            }

            int maxParts = Mathf.Clamp(valuesToShow, 1, 5);
            if (maxParts > formattedCount)
            {
                maxParts = formattedCount;
            }

            int firstNonZeroFormattedIndex = -1;
            int lastNonZeroFormattedIndex = -1;

            for (int idx = 0; idx <= 4; idx++)
            {
                if (!HasFormat(idx))
                {
                    continue;
                }

                int value = GetValueByIndex(idx, days, hours, minutes, seconds, milliseconds);
                if (value != 0)
                {
                    if (firstNonZeroFormattedIndex == -1)
                    {
                        firstNonZeroFormattedIndex = idx;
                    }

                    lastNonZeroFormattedIndex = idx;
                }
            }

            bool isZeroLike = firstNonZeroFormattedIndex == -1;

            if (isZeroLike)
            {
                int formattedIndicesCount = 0;
                for (int idx = 0; idx <= 4; idx++)
                {
                    if (!HasFormat(idx))
                    {
                        continue;
                    }

                    buffer[formattedIndicesCount].Index = idx;
                    formattedIndicesCount++;
                }

                int start = formattedIndicesCount - maxParts;
                if (start < 0)
                {
                    start = 0;
                }

                stringBuilder.Length = 0;
                for (int i = start; i < formattedIndicesCount; i++)
                {
                    int idx = buffer[i].Index;
                    VariableBinding<string> binding = GetBindingByIndex(idx);
                    string text = FormatUnit(binding, 0);
                    if (string.IsNullOrEmpty(text))
                    {
                        continue;
                    }

                    stringBuilder.Append(text);
                }
                
                exposedProperty.Value = stringBuilder.ToString();
                return;
            }

            int lastFormattedIndex = -1;
            for (int idx = 0; idx <= 4; idx++)
            {
                if (HasFormat(idx))
                {
                    lastFormattedIndex = idx;
                }
            }

            bool bottomAligned = firstNonZeroFormattedIndex == lastNonZeroFormattedIndex && lastNonZeroFormattedIndex == lastFormattedIndex;

            int partsCount = 0;

            if (bottomAligned)
            {
                for (int idx = lastFormattedIndex; idx >= 0 && partsCount < maxParts; idx--)
                {
                    if (!HasFormat(idx))
                    {
                        continue;
                    }

                    buffer[partsCount].Index = idx;
                    partsCount++;
                }

                for (int i = 0; i < partsCount / 2; i++)
                {
                    (buffer[i].Index, buffer[partsCount - 1 - i].Index) = (buffer[partsCount - 1 - i].Index, buffer[i].Index);
                }
            }
            else
            {
                for (int idx = firstNonZeroFormattedIndex; idx <= 4 && partsCount < maxParts; idx++)
                {
                    if (!HasFormat(idx))
                    {
                        continue;
                    }

                    buffer[partsCount].Index = idx;
                    partsCount++;
                }
            }

            for (int i = 0; i < partsCount; i++)
            {
                int idx = buffer[i].Index;
                int value = GetValueByIndex(idx, days, hours, minutes, seconds, milliseconds);
                VariableBinding<string> binding = GetBindingByIndex(idx);
                string text = FormatUnit(binding, value);
                buffer[i].Value = value;
                buffer[i].Text = text ?? string.Empty;
            }

            if (trimLeadingZeros)
            {
                int shift = 0;
                while (shift < partsCount - 1 && buffer[shift].Value == 0)
                {
                    shift++;
                }

                if (shift > 0)
                {
                    int newCount = partsCount - shift;
                    for (int i = 0; i < newCount; i++)
                    {
                        buffer[i] = buffer[i + shift];
                    }

                    partsCount = newCount;
                }
            }

            stringBuilder.Length = 0;
            for (int i = 0; i < partsCount; i++)
            {
                stringBuilder.Append(buffer[i].Text);
            }

            exposedProperty.Value = stringBuilder.ToString();
        }

        private bool HasFormat(int index)
        {
            switch (index)
            {
                case 0:
                    return !string.IsNullOrEmpty(daysFormatBinding.Property.Value);
                case 1:
                    return !string.IsNullOrEmpty(hoursFormatBinding.Property.Value);
                case 2:
                    return !string.IsNullOrEmpty(minutesFormatBinding.Property.Value);
                case 3:
                    return !string.IsNullOrEmpty(secondsFormatBinding.Property.Value);
                case 4:
                    return !string.IsNullOrEmpty(millisecondsFormatBinding.Property.Value);
                default:
                    return false;
            }
        }

        private int GetValueByIndex(int index, int days, int hours, int minutes, int seconds, int milliseconds)
        {
            switch (index)
            {
                case 0:
                    return days;
                case 1:
                    return hours;
                case 2:
                    return minutes;
                case 3:
                    return seconds;
                case 4:
                    return milliseconds;
                default:
                    return 0;
            }
        }

        private VariableBinding<string> GetBindingByIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return daysFormatBinding;
                case 1:
                    return hoursFormatBinding;
                case 2:
                    return minutesFormatBinding;
                case 3:
                    return secondsFormatBinding;
                case 4:
                    return millisecondsFormatBinding;
                default:
                    return null;
            }
        }

        private string FormatUnit(VariableBinding<string> binding, int value)
        {
            if (binding == null)
            {
                return null;
            }

            var prop = binding.Property;
            var format = prop.Value;
            if (string.IsNullOrEmpty(format))
            {
                return null;
            }

            return string.Format(format, value);
        }

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();
            SanitizeTypes();
        }

        private void SanitizeTypes()
        {
            bool didTypeChange = false;

            if (expectedType?.Type == null)
            {
                expectedType = new SerializableType(InjectionType);
                didTypeChange = true;
            }

            if (didTypeChange)
            {
                BindingInfoTrackerProxy.RefreshBindingInfo();
            }
        }
#endif
    }
}
