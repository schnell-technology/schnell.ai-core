using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Schnell.Ai.Shared.Helper
{
    /// <summary>
    /// Map a object to a dictionary and vice-versa
    /// </summary>
    public static class ObjectDictionaryMapper
    {
        /// <summary>
        /// Create a new object by dictionary
        /// </summary>
        /// <typeparam name="TObject">Type of object</typeparam>
        /// <param name="d">Dictionary</param>
        /// <returns>newly created object of TObject</returns>
        public static TObject GetObject<TObject>(IDictionary<string, object> d)
        {
            return (TObject)GetObject(typeof(TObject), d);
        }

        /// <summary>
        /// Create a new object by dictionary
        /// </summary>
        /// <param name="type">Type of object</param>
        /// <param name="d">Dictionary</param>
        /// <returns>newly created object of Type</returns>
        public static object GetObject(Type type, IDictionary<string, object> d)
        {
            PropertyInfo[] props = type.GetProperties();
            object res = Activator.CreateInstance(type);
            for (int i = 0; i < props.Length; i++)
            {
                if (props[i].CanWrite && d.ContainsKey(props[i].Name))
                {
                    props[i].SetValue(res, d[props[i].Name], null);
                }
            }
            return res;
        }

        /// <summary>
        /// Create dictionary out of a object-instance
        /// </summary>
        /// <typeparam name="TObject">Type of object</typeparam>
        /// <param name="o">Object</param>
        /// <returns>Dictionary</returns>
        public static IDictionary<string, object> GetDictionary<TObject>(TObject o)
        {
            return GetDictionary(o);
        }

        private static void GetDictionary(Dictionary<string,object> dictionary, object o, string nestedName = null)
        {
            if (o is IDictionary)
            {
                var valDict = (o as IDictionary).Keys.Cast<object>().ToDictionary(k => k.ToString(), v => (o as IDictionary)[v]);
                valDict.ToList().ForEach(kvP => {
                    var key = kvP.Key;
                    if (nestedName != null)
                        key = nestedName + "." + kvP.Key;

                    AddValue(dictionary, key, kvP.Value);
                });
            }
            else
            {
                var properties = o.GetType().GetProperties();
                foreach (var p in properties)
                {
                    var key = p.Name;
                    if (nestedName != null)
                        key = nestedName + "." + p.Name;

                    object value = p.GetValue(o, null);

                    AddValue(dictionary, key, value);                    
                }
            }
        }

        private static void AddValue(Dictionary<string,object>dictionary , string key, object value)
        {
            Type valueType = value.GetType();
            if (valueType.IsPrimitive || valueType == typeof(String))
            {
                dictionary[key] = value.ToString();
            }
            else if (value is IEnumerable)
            {
                var i = 0;
                foreach (object oEnum in (IEnumerable)value)
                {
                    GetDictionary(dictionary, oEnum, key + "[" + i + "]");
                    i++;
                }
            }
            else if (value is IDictionary)
            {
                GetDictionary(dictionary, value, key);
            }
            else
            {
                GetDictionary(dictionary, value, key);
            }
        }
    }
}
