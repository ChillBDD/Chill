using System;

namespace Chill
{
    /// <summary>
    /// Defines which Chill Test initializer (and which container) to use for an Assembly or Class. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class)]
    public class ChillContainerInitializerAttribute : Attribute
    {
        /// <summary>
        /// The type of test initializer to user. 
        /// </summary>
        public Type ChillContainerInitializerType { get; private set; }

        /// <summary>
        /// Assigns a Chill Test Initializer to a class / assembly
        /// </summary>
        /// <param name="ChillContainerInitializerType"></param>
        public ChillContainerInitializerAttribute(Type chillContainerInitializerType)
        {
            ChillContainerInitializerType = chillContainerInitializerType;
        }
    }
}