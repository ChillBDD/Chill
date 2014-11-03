using System;

namespace Chill
{
    /// <summary>
    /// Defines which Chill Test initializer (and which container) to use for an Assembly or Class. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class)]
    public class ChillTestInitializerAttribute : Attribute
    {
        /// <summary>
        /// The type of test initializer to user. 
        /// </summary>
        public Type ChillTestInitializerType { get; private set; }

        /// <summary>
        /// Assigns a Chill Test Initializer to a class / assembly
        /// </summary>
        /// <param name="chillTestInitializerType"></param>
        public ChillTestInitializerAttribute(Type chillTestInitializerType)
        {
            ChillTestInitializerType = chillTestInitializerType;
        }
    }
}