﻿namespace Mace
{
	public interface IBinding
	{
		bool IsBound { get; }
		void Bind();
		void Unbind();
	}
}