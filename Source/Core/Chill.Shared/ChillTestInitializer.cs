using System;
using System.ComponentModel;

namespace Chill
{
    public class ChillContainerAttribute : ChillContainerInitializerAttribute
    {
        public ChillContainerAttribute(Type containerType)
            : base(typeof(DefaultChillContainerInitializer<>).MakeGenericType(containerType))
        {
            
        }

        
    }
}