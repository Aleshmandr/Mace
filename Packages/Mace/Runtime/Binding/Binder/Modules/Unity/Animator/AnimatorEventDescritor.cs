using System;

namespace Mace
{
    [Serializable]
    public class AnimatorEventDescritor
    {
        public string Name;
        public BindingInfo CommandBinding = BindingInfo.Command();
    }
}