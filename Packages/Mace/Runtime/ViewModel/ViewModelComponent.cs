﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mace
{
	public abstract class ViewModelComponent<T> : MonoBehaviour, IViewModelProvider<T> where T : IViewModel
	{
		[TypeConstraint(typeof(IViewModel), true)]
		[SerializeField, HideInInspector] protected SerializableType expectedType;
		[SerializeField, DisableAtRuntime] private string id;
		
		public event ViewModelChangeEventHandler<T> ViewModelChanged;

		public virtual Type ExpectedType => expectedType.Type;

		public T ViewModel
		{
			get => viewModel;
			set
			{
				T lastViewModel = viewModel;
				viewModel?.Disable();
				viewModel = value;
				viewModel?.Enable();
				OnViewModelChanged(lastViewModel, viewModel);
			}
		}

		public string Id => id;

		private T viewModel;

		protected virtual void Reset()
		{
			ResetId();
		}

		protected virtual void OnValidate()
		{
			EnsureId();
		}

		protected virtual void Awake()
		{
			EnsureId();
		}

		protected virtual void OnViewModelChanged(T lastViewModel, T newViewModel)
		{
			ViewModelChanged?.Invoke(this, lastViewModel, newViewModel);
		}
		
		private void EnsureId()
		{
			if (string.IsNullOrEmpty(id) || IsIdAvailable(id) == false)
			{
				ResetId();
			}
		}

		private void ResetId()
		{
			string candidate = GetNewId();

			while (IsIdAvailable(candidate) == false)
			{
				candidate = GetNewId();
			}

			id = candidate;
		}

		private string GetNewId()
		{
			return Guid.NewGuid().ToString().Substring(0, 4);
		}

		private bool IsIdAvailable(string candidate)
		{
			IEnumerable<ViewModelComponent<IViewModel>> components =
				GetComponentsInChildren<Component>(true)
					.Concat(GetComponentsInParent<Component>())
					.Where(x => x is ViewModelComponent<IViewModel>)
					.Cast<ViewModelComponent<IViewModel>>();
			bool result = IsIdAvailable(candidate, components);
			return result;
		}

		private bool IsIdAvailable(string candidate, IEnumerable<ViewModelComponent<IViewModel>> components)
		{
			var existingId = components.FirstOrDefault(x => x != this && string.Equals(candidate, x.id));

			if (existingId)
			{
				Debug.LogError($"Id \"{candidate}\" already taken by {existingId.name}", this);
			}

			return !existingId;
		}
	}

	public class ViewModelComponent : ViewModelComponent<IViewModel>
	{
		protected override void Awake()
		{
			base.Awake();
			ViewModelComponentTree.Register(this);
		}

		protected virtual void OnDestroy()
		{
			ViewModelComponentTree.Unregister(this);
		}

		protected virtual void OnTransformParentChanged()
		{
			ViewModelComponentTree.Move(this);
		}
	}
}
