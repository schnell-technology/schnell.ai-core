using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Linq;

namespace Schnell.Ai.Shared.Helper
{
    /// <summary>
    /// Extensions for type-mapping and converting
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Create a ExpandoObject out of a dictionary
        /// </summary>
        /// <param name="dictionary">Dictionary of string and object</param>
        /// <returns>Created Expando-Object</returns>
        public static ExpandoObject ToExpandoObject(this IDictionary<string, object> dictionary)
        {
            var expando = new ExpandoObject();
            var expandoDic = (IDictionary<string, object>)expando;

            // go through the items in the dictionary and copy over the key value pairs)
            foreach (var kvp in dictionary)
            {
                // if the value can also be turned into an ExpandoObject, then do it!
                if (kvp.Value is IDictionary<string, object>)
                {
                    var expandoValue = ((IDictionary<string, object>)kvp.Value).ToExpandoObject();
                    expandoDic.Add(kvp.Key, expandoValue);
                }
                else if (kvp.Value is ICollection)
                {
                    // iterate through the collection and convert any strin-object dictionaries
                    // along the way into expando objects
                    var itemList = new List<object>();
                    foreach (var item in (ICollection)kvp.Value)
                    {
                        if (item is IDictionary<string, object>)
                        {
                            var expandoItem = ((IDictionary<string, object>)item).ToExpandoObject();
                            itemList.Add(expandoItem);
                        }
                        else
                        {
                            itemList.Add(item);
                        }
                    }

                    expandoDic.Add(kvp.Key, itemList);
                }
                else
                {
                    expandoDic.Add(kvp);
                }
            }

            return expando;
        }

        /// <summary>
        /// Create a newly compiled typed object out of a directory
        /// </summary>
        /// <param name="prototype">Directory</param>
        /// <param name="typeSignature">Name of type</param>
        /// <returns>New object</returns>
        public static object CreateTypedObject(Dictionary<string, object> prototype, string typeSignature = null)
        {
            if(!String.IsNullOrEmpty(typeSignature))
            {
                typeSignature = "_DynamicTypes.Type-" + Guid.NewGuid();
            }
            var myTypeInfo = CreateNewType(prototype, typeSignature);
            var myType = myTypeInfo.AsType();
            var myObject = Activator.CreateInstance(myType);

            return myObject;
        }

        /// <summary>
        /// Create a new object-instance of a given type
        /// </summary>
        /// <param name="type">Type to be created</param>
        /// <returns>New object-instance</returns>
        public static object CreateTypedObject(Type type)
        {
            var myObject = Activator.CreateInstance(type);
            return myObject;
        }

        /// <summary>
        /// Create a new enumerable of a specified type
        /// </summary>
        /// <param name="type">Specified type</param>
        /// <returns>Enumerable of specified type</returns>
        public static IEnumerable MakeEnumerable(Type type)
        {
            var listType = typeof(IEnumerable<>);
            var constructedListType = listType.MakeGenericType(type);

            var instance = Activator.CreateInstance(constructedListType);
            return (IEnumerable)instance;
        }

        /// <summary>
        /// Create a new IList of a specified type
        /// </summary>
        /// <param name="type">Specified type</param>
        /// <returns>New list of specified type</returns>
        public static IList MakeList(Type type)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(type);

            var instance = Activator.CreateInstance(constructedListType);
            return (IList)instance;
        }

        /// <summary>
        /// Compile a new type of a prototype-dictionary
        /// </summary>
        /// <param name="prototype">Dictionary</param>
        /// <param name="typeSignature">Name of type</param>
        /// <returns>TypeInfo for newly compiled type</returns>
        public static TypeInfo CreateNewType(Dictionary<string, object> prototype, string typeSignature)
        {
            TypeBuilder tb = GetTypeBuilder(typeSignature);    
            
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            Dictionary<string, Type> listOfFields = prototype.Where(kv => kv.Value != null).ToDictionary((o) => o.Key, (v) => v.Value.GetType());
                        
            foreach (var field in listOfFields)
                CreateProperty(tb, field.Key, field.Value);

            TypeInfo objectTypeInfo = tb.CreateTypeInfo();
            return objectTypeInfo;
        }

        private static TypeBuilder GetTypeBuilder(string typeSignature)
        {
            var an = new AssemblyName(typeSignature);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    null);
            return tb;
        }

        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    }
}
