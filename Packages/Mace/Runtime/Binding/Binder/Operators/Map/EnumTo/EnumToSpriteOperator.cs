using System;
using Mace.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Mace
{
    public class EnumToSpriteOperator<T> : MapOperator<T, Sprite> where T : Enum
    {
    }
}
