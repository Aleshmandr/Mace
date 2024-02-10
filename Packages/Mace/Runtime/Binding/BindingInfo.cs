﻿using System;
using UnityEngine;

namespace Mace
{
    [Serializable]
    public class BindingInfo
    {
        public Type Type => type.Type;

        public ViewModelComponent ViewModelContainer
        {
            get => viewModelContainer;
            set => viewModelContainer = value;
        }

        public string Path => path;
        public bool ForceDynamicBinding => forceDynamicBinding;

        [SerializeField] protected SerializableType type;
        [SerializeField] private ViewModelComponent viewModelContainer;
        [SerializeField] private string path;
        [SerializeField] private bool forceDynamicBinding;

        public BindingInfo(Type targetType)
        {
            type = new SerializableType(targetType);
        }

        public static BindingInfo Variable<T>()
        {
            return new BindingInfo(typeof(IReadOnlyObservableVariable<T>));
        }

        public static BindingInfo Collection<T>()
        {
            return new BindingInfo(typeof(IReadOnlyObservableCollection<T>));
        }

        public static BindingInfo Command()
        {
            return new BindingInfo(typeof(IObservableCommand));
        }

        public static BindingInfo Command<T>()
        {
            return new BindingInfo(typeof(IObservableCommand<T>));
        }

        public static BindingInfo Event()
        {
            return new BindingInfo(typeof(IObservableEvent));
        }

        public static BindingInfo Event<T>()
        {
            return new BindingInfo(typeof(IObservableEvent<T>));
        }
    }

    [Serializable]
    public class BindingInfo<T> : BindingInfo
    {
        public BindingInfo() : base(typeof(T)) { }
    }
}
