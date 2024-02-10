using System;
using UnityEngine;

namespace Mace
{
    public abstract class EnumToColorOperator<T> : MapOperator<T, Color> where T : Enum
    {
    }
}
