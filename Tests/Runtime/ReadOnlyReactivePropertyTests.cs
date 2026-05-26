using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mace.Tests
{
    public class ReadOnlyReactivePropertyTests
    {
        [UnityTest]
        public IEnumerator Select_WhenSourceChanges_UpdatesValue()
        {
            ObservableVariable<int> source = new ObservableVariable<int>(2);
            ReadOnlyReactiveProperty<string> property = source.Select(value => value.ToString());

            Assert.IsTrue(property.HasValue);
            Assert.AreEqual("2", property.Value);

            source.Value = 5;

            Assert.AreEqual("5", property.Value);

            yield return null;
        }

        [UnityTest]
        public IEnumerator CombineLatest_WithDifferentSourceTypes_UpdatesValue()
        {
            ObservableVariable<bool> isEnabled = new ObservableVariable<bool>(true);
            ObservableVariable<float> timer = new ObservableVariable<float>(1f);
            ReadOnlyReactiveProperty<bool> property = isEnabled.CombineLatest(timer, (enabled, time) => enabled && time <= 0f);

            int callbackCount = 0;
            bool lastValue = false;
            property.Changed += value =>
            {
                callbackCount++;
                lastValue = value;
            };

            timer.Value = 0f;

            Assert.AreEqual(1, callbackCount);
            Assert.IsTrue(lastValue);

            yield return null;
        }

        [UnityTest]
        public IEnumerator CombineLatest_WhenResultDoesNotChange_DoesNotRaiseCallback()
        {
            ObservableVariable<int> current = new ObservableVariable<int>(1);
            ObservableVariable<int> required = new ObservableVariable<int>(3);
            ReadOnlyReactiveProperty<bool> property = current.CombineLatest(required, (currentValue, requiredValue) =>
                currentValue >= requiredValue);

            int callbackCount = 0;
            property.Changed += _ => callbackCount++;

            current.Value = 2;

            Assert.AreEqual(0, callbackCount);

            yield return null;
        }

        [UnityTest]
        public IEnumerator SourceClear_WhenPropertyHadValue_RaisesCleared()
        {
            ObservableVariable<int> source = new ObservableVariable<int>(2);
            ReadOnlyReactiveProperty<bool> property = source.Select(value => value > 0);

            bool wasCleared = false;
            property.Cleared += () => wasCleared = true;

            source.Clear();

            Assert.IsTrue(wasCleared);
            Assert.IsFalse(property.HasValue);

            yield return null;
        }
    }
}
