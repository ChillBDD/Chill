using System;

namespace Chill
{

    /// <summary>
    /// Interface for automatic mothers. Classes with this interface will be discovered automatically to construct 
    /// objects automatically. 
    /// </summary>
    public interface IAutoMother
    {

        /// <summary>
        /// Checks if this automother is applicable to the specified type. 
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        bool Applies(Type type);


        /// <summary>
        /// Creates the specified type using the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container">The container.</param>
        /// <returns>A created instance of the passed type</returns>
        T Create<T>(IChillContainer container);
    }
}