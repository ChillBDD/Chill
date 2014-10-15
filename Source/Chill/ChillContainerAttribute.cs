using System;

namespace Chill
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class)]
    public class ChillContainerAttribute : Attribute
    {
        public Type ChillContainerType { get; private set; }

        public ChillContainerAttribute(Type chillContainerType)
        {
            ChillContainerType = chillContainerType;
        }
    }
}