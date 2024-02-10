using System;

namespace Mace
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PathAttribute : Attribute
    {
        public string Path { get; private set; }

        public PathAttribute(string path)
        {
            Path = path;
        }
    }
}
