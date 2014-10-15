using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Chill
{
    /// <summary>
    /// Class that helps to resolve assembly types. If an error occurs while loading assembly types, it will catch and continue
    /// 
    /// This class was adapted from the DefaultReflector from FluentAssertions by Dennis Doomen.
    /// https://github.com/dennisdoomen/fluentassertions/blob/develop/FluentAssertions.Net40/Common/DefaultReflectionProvider.cs
    /// </summary>
    internal static class AssemblyTypeResolver
    {
        public static IEnumerable<Type> GetAllTypesFromAssemblies(IEnumerable<Assembly> assemblies )
        {
            return assemblies
                .Where(a => !IsDynamic(a))
                .SelectMany(GetExportedTypes).ToArray();
        }

        private static bool IsDynamic(Assembly assembly)
        {
            return (assembly is AssemblyBuilder) ||
                   (assembly.GetType().FullName == "System.Reflection.Emit.InternalAssemblyBuilder");
        }

        private static IEnumerable<Type> GetExportedTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types;
            }
            catch (FileLoadException)
            {
                return new Type[0];
            }
            catch (Exception)
            {
                return new Type[0];
            }
        }
    }
}