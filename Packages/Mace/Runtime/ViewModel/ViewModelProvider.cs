﻿using System;
using UnityEngine;

namespace Mace
{
    [RequireComponent(typeof(ViewModelComponent))]
    public class ViewModelProvider<T> : MonoBehaviour, IViewModelInjector, IViewModelProvider<T> where T : IViewModel
    {
        public Type InjectionType => typeof(T);
        public ViewModelComponent Target => targetComponent;
        public T ViewModel => (T)(Target ? Target.ViewModel : default);

        [SerializeField, HideInInspector] private ViewModelComponent targetComponent;
        public event ViewModelChangeEventHandler<T> ViewModelChanged;

        protected virtual void Awake()
        {
            RetrieveRequiredComponents();
            Target.ViewModelChanged += OnTargetViewModelChanged;
        }

        protected virtual void OnDestroy()
        {
            Target.ViewModelChanged -= OnTargetViewModelChanged;
        }

        protected virtual void Reset()
        {
            RetrieveRequiredComponents();
        }
        
        public virtual void SetViewModel(IViewModel viewModel)
        {
            if (viewModel == null)
            {
                return;
            }
            
            if (viewModel is T typedViewModel)
            {
                SetViewModel(typedViewModel);
            }
            else
            {
                Debug.LogError($"ViewModel passed have wrong type! ({viewModel.GetType()} instead of {typeof(T)})", this);
            }
        }
        
        protected virtual void SetViewModel(T viewModel)
        {
            if (targetComponent != null)
            {
                targetComponent.ViewModel = viewModel;
            }
        }
        
        protected virtual void OnViewModelChanged(T lastViewModel, T newViewModel)
        {
            ViewModelChanged?.Invoke(this, lastViewModel, newViewModel);
        }

        private void RetrieveRequiredComponents()
        {
            if (!targetComponent)
            {
                targetComponent = GetComponent<ViewModelComponent>();
            }
        }

        private void OnTargetViewModelChanged(IViewModelProvider<IViewModel> source, IViewModel lastViewModel, IViewModel newViewModel)
        {
            OnViewModelChanged((T)lastViewModel, (T)newViewModel);
        }
    }
}