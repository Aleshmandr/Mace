using System;

namespace Mace
{
    public abstract class EnumToBoolOperator<T> : MapOperator<T, bool> where T : Enum
    {
    }
}
