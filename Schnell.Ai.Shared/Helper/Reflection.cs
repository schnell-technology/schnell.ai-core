using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schnell.Ai.Shared.Helper
{
    public static class Reflection
    {
        /// <summary>
        /// Make a method generic and invoke the method
        /// </summary>
        /// <typeparam name="T">Type of object-instance</typeparam>
        /// <param name="instance">Object-instance</param>
        /// <param name="methodName">Name of method</param>
        /// <param name="genericType">Generic type to be used in execution</param>
        /// <param name="parameters">Optional parameters for the method to be called</param>
        /// <returns>Returned result</returns>
        public static object MakeGenericAndInvoke<T>(
            T instance,
            string methodName,
            Type genericType,
            params object[] parameters
            )
        {
            var loadMethodGeneric = typeof(T).GetMethods().First(
                method => method.Name == methodName && method.IsGenericMethod && method.GetParameters().Count() == parameters.Count()
                );
            var loadMethod = loadMethodGeneric.MakeGenericMethod(genericType);
            return loadMethod.Invoke(instance, parameters);
        }

        /// <summary>
        /// Make a method generic (multiple Types) and invoke the method
        /// </summary>
        /// <typeparam name="T">Type of object-instance</typeparam>
        /// <param name="instance">Object-instance</param>
        /// <param name="methodName">Name of method</param>
        /// <param name="genericTypes">Generic types to be used in execution</param>
        /// <param name="parameters">Optional parameters for the method to be called</param>
        /// <returns>Returned result</returns>
        public static object MakeGenericAndInvoke<T>(
            T instance,
            string methodName,
            Type[] genericTypes,
            params object[] parameters
            )
        {
            var loadMethodGeneric = typeof(T).GetMethods().First(
                method => method.Name == methodName && method.IsGenericMethod && method.GetParameters().Count() == parameters.Count()
                );
            var loadMethod = loadMethodGeneric.MakeGenericMethod(genericTypes);
            return loadMethod.Invoke(instance, parameters);
        }
    }
}
