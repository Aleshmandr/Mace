using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mace.Utils.Singleton
{
    /// <summary>
    /// The singleton interface.
    /// </summary>
    public interface ISingleton
    {
        public void InitializeSingleton();
        public void ClearSingleton();
    }
}