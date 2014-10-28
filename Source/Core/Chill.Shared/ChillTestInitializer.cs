using System;

namespace Chill
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class)]
    public class ChillTestInitializerAttribute : Attribute
    {
        public Type ChillTestContextType { get; private set; }

        public ChillTestInitializerAttribute(Type chillContainerType)
        {
            ChillTestContextType = chillContainerType;
        }
    }
}