using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schnell.Ai.Shared.Helper
{
    public static class Enumerable
    {
        /// <summary>
        /// Cast enumerable to a specified type
        /// </summary>
        /// <param name="enumerable">Enumerable of objects</param>
        /// <param name="castType">Type to be casted</param>
        /// <returns>Casted enumerable of type 'castType'</returns>
        public static object CastEnumerable(IEnumerable<object> enumerable, Type castType)
        {
            var casted = typeof(System.Linq.Enumerable).GetMethod(nameof(System.Linq.Enumerable.Cast)).MakeGenericMethod(new[] { castType });
            return casted.Invoke(null, new[] { enumerable });
        }
    }
}
