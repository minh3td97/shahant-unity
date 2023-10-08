using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;
using Object = UnityEngine.Object;

public static class ReflectionUtils
{

    public static BindingFlags CommonBindings => BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public static string[] GetStaticStringFieldValues(Type type)
    {
        var names = new List<string>();
        var properties = type.GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var info in properties)
        {
            if (info.FieldType == typeof(string))
            {
                names.Add((string)info.GetValue(null));
            }
        }

        var query = names.GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(y => y.Key)
            .ToList();
        if (query.Count > 0)
        {
            Debug.LogErrorFormat("Duplicate Keys {0}", query.Aggregate("", (i, j) => i + " " + j));
            return new string[0];
        }
        else
        {
            return names.ToArray();
        }
    }

    public static List<string> GetInstanceFields<T, K>(bool includeNonPublic = true, bool inherit = true)
    {
        List<string> names = new List<string>();
        var flags = BindingFlags.Instance | BindingFlags.Public;
        if (includeNonPublic) flags |= BindingFlags.NonPublic;

        var fields = typeof(K).GetFields(flags);
        foreach (var f in fields)
        {
            if (inherit && typeof(T).IsAssignableFrom(f.FieldType))
            {
                names.Add(f.Name);
            }
            else if (typeof(T) == f.FieldType)
            {
                names.Add(f.Name);
            }
        }

        return names;
    }

    public static List<string> GetFields(Type type, Type fieldType, bool includeNonPublic = true, bool covariance = false)
    {
        List<string> names = new List<string>();
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;
        if (includeNonPublic) flags |= BindingFlags.NonPublic;

        var fields = type.GetFields(flags).Where(i => !i.IsSpecialName);
        foreach (var f in fields)
        {
            if ((covariance && fieldType.IsAssignableFrom(f.FieldType))
                || (!covariance && f.FieldType.IsAssignableFrom(fieldType))
               )
            {
                names.Add(f.Name);
            }
        }

        return names;
    }

    public static List<string> GetInstanceFields(Type type, Type fieldType, bool includeNonPublic = true, bool covariance = false)
    {
        List<string> names = new List<string>();
        var flags = BindingFlags.Instance | BindingFlags.Public;
        if (includeNonPublic) flags |= BindingFlags.NonPublic;

        var fields = type.GetFields(flags).Where(i => !i.IsSpecialName);
        foreach (var f in fields)
        {
            if ((covariance && fieldType.IsAssignableFrom(f.FieldType))
                || (!covariance && f.FieldType.IsAssignableFrom(fieldType))
               )
            {
                names.Add(f.Name);
            }
        }

        return names;
    }

    public static List<string> GetProperties(Type type, Type propertyType, bool includeNonPublic = true, bool covariance = false, bool canWrite = true)
    {
        List<string> names = new List<string>();
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;
        if (includeNonPublic) flags |= BindingFlags.NonPublic;

        var fields = type.GetProperties(flags);
        foreach (var f in fields)
        {
            if ((covariance && propertyType.IsAssignableFrom(f.PropertyType))
                || (!covariance && f.PropertyType.IsAssignableFrom(propertyType)))
            {
                if (!canWrite || f.CanWrite)
                {
                    names.Add(f.Name);
                }
            }
        }

        return names;
    }

    public static List<string> GetInstanceProperties(Type type, Type propertyType, bool includeNonPublic = true, bool covariance = false, bool canWrite = true)
    {
        List<string> names = new List<string>();
        var flags = BindingFlags.Instance | BindingFlags.Public;
        if (includeNonPublic) flags |= BindingFlags.NonPublic;

        var fields = type.GetProperties(flags);
        foreach (var f in fields)
        {
            if ((covariance && propertyType.IsAssignableFrom(f.PropertyType))
                || (!covariance && f.PropertyType.IsAssignableFrom(propertyType)))
            {
                if (!canWrite || f.CanWrite)
                {
                    names.Add(f.Name);
                }
            }
        }

        return names;
    }

    public static List<string> GetInstanceMembers(Type type, Type memberType, bool includeNonPublic = true, bool covariance = false, bool canWrite = true)
    {
        return GetInstanceFields(type, memberType, includeNonPublic, covariance).Concat(GetInstanceProperties(type, memberType, includeNonPublic, covariance, canWrite)).ToList();
    }

    public static List<string> GetInstanceMethods(Type type, Type returnType, bool includeNonPublic = true)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public;
        if (includeNonPublic) flags |= BindingFlags.NonPublic;

        var methods = type.GetMethods(flags);
        return methods.Where(i => i.ReturnType != typeof(void)
                                && returnType.IsAssignableFrom(i.ReturnType)
                                && i.GetParameters().Length == 0
                                && !i.IsSpecialName).Select(i => i.Name).ToList();
    }

    public static List<string> GetInstanceMethods(Type type, bool includeNonPublic = true)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public;
        if (includeNonPublic) flags |= BindingFlags.NonPublic;

        var methods = type.GetMethods(flags);
        return methods.Where(i => i.GetParameters().Length == 0
                                && !i.IsSpecialName).Select(i => i.Name).ToList();
    }

    public static List<string> GetInstanceMethodsWithParameter(Type type, Type parameterType, bool includeNonPublic = true)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public;
        if (includeNonPublic) flags |= BindingFlags.NonPublic;

        var methods = type.GetMethods(flags);
        return methods.Where(i => i.GetParameters().Length == 1
                                && parameterType.IsAssignableFrom(i.GetParameters()[0].ParameterType)
                                && !i.IsSpecialName).Select(i => i.Name).ToList();
    }

    public static List<string> GetNoParameterInstanceMethods(Type type, bool includeNonPublic = true)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public;
        if (includeNonPublic) flags |= BindingFlags.NonPublic;

        var methods = type.GetMethods(flags);
        return methods.Where(i => i.GetParameters().Length == 0
                                && !i.IsSpecialName).Select(i => i.Name).ToList();
    }

    public static List<string> Methods(this Type type)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        return type.GetMethods(flags).Where(i => !i.IsSpecialName).Select(i => /* string.Format("{0} (static={1}, public={2}) , i.Name, i.IsStatic, i.IsPublic) */ i.GetMethodSignature()).ToList();
    }

    public static List<string> Properties(this Type type)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        return type.GetProperties(flags).Select(i => string.Format("{0} ({1}, setter={2})", i.Name, i.PropertyType, i.SetMethod != null)).ToList();
    }

    public static List<string> Fields(this Type type)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        return type.GetFields(flags).Select(i => string.Format("{0} ({1}, static={2}, public={3})", i.Name, i.FieldType, i.IsStatic, i.IsPublic)).ToList();
    }

    public static object GetFieldValue(this object obj, string field)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        var f = obj.GetType().GetField(field, flags);
        return f.GetValue(obj);
    }

    public static void SetFieldValue(this object obj, string field, object value)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        var f = obj.GetType().GetField(field, flags);
        f.SetValue(obj, value);
    }

    public static object GetFieldOrPropertyValue(this object source, string name)
    {
        if (source == null) return null;
        var type = source.GetType();
        var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (f != null) return f.GetValue(source);

        var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        return p == null ? null : p.GetValue(source, null);
    }

    public static object GetFieldOrPropertyValue(this object source, string name, int index)
    {
        var enumerable = GetFieldOrPropertyValue(source, name) as IEnumerable;
        var enm = enumerable.GetEnumerator();
        while (index-- >= 0)
            enm.MoveNext();
        return enm.Current;
    }

    public static void CallMethod(this object obj, string method, params object[] data)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        var f = obj.GetType().GetMethod(method, flags);
        f.Invoke(obj, data);
    }

    public static string GetMethodSignature(this MethodInfo mi)
    {
        string[] param = mi.GetParameters()
                              .Select(p => string.Format("{0} {1}", p.ParameterType.Name, p.Name))
                              .ToArray();


        string signature = string.Format("{0} {1}({2})", mi.ReturnType.Name, mi.Name, string.Join(",", param));

        return signature;
    }

    public static List<(PropertyInfo property, T attribute)> GetInstanceAttributedProperties<T>(this Type type) where T : Attribute
    {
        return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Select(i => (property: i, attribute: i.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T))
                    .Where(i => i.attribute != null)
                    .ToList();
    }

    public static List<(FieldInfo field, T attribute)> GetInstanceAttributedFields<T>(this Type type) where T : Attribute
    {
        return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Select(i => (field: i, attribute: i.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T))
                    .Where(i => i.attribute != null)
                    .ToList();
    }

    public static Type GetGenericType(this Type type, Type genericDefinitionType)
    {
        while (true)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericDefinitionType) return type;

            type = type.BaseType;
            if (type == null) break;
        }

        return null;
    }

    public static bool IsOfGenericType(this Type type, Type genericType)
    {
        return GetGenericType(type, genericType) != null;
    }

    public static Type[] GetGenericArguments(this Type type, Type genericType)
    {
        return GetGenericType(type, genericType)?.GetGenericArguments();
    }

    public static Type GetGenericArgument(this Type type, Type genericType)
    {
        return GetGenericArguments(type, genericType)?[0];
    }

    public static bool IsDerivedFrom(this Type type, Type baseType) => baseType.IsAssignableFrom(type);
    public static bool IsBasedFrom(this Type type, Type derivedType) => type.IsAssignableFrom(derivedType);

    public static IEnumerable<Type> GetDerived(this Type baseType, bool includeAbstract = false)
        => GetAllTypes().Where(i => baseType.IsAssignableFrom(i) && (includeAbstract || !i.IsAbstract));

    public static IEnumerable<Type> GetAllTypes<TBaseType>(bool includeAbstract = false)
        => GetAllTypes().Where(i => typeof(TBaseType).IsAssignableFrom(i) && (includeAbstract || !i.IsAbstract));


    public static Type GetType(string name) => GetAllTypes().FirstOrDefault(i => i.FullName.Equals(name));

    public static IEnumerable<Type> GetAllTypes(Type baseType)
    {
        foreach (var t in GetAllTypes())
        {
            if (baseType.IsAssignableFrom(t)) yield return t;
        }
        //return GetAllTypes().Where(i => baseType.IsAssignableFrom(i));
    }

    static List<Type> AllTypes;
    public static List<Type> GetAllTypes()
    {
        if (AllTypes != null) return AllTypes;

        var res = new List<Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            res.AddRange(assembly.GetTypes());
        }

        var x = new Func<string, bool>(name => !name.Contains("system.") && !name.Contains("mono.") &&
                                                !name.Contains("mono.") && !name.Contains("icsharpcode.") &&
                                                !name.Contains("nsubstitute") && !name.Contains("nunit.") &&
                                                !name.Contains("microsoft.") && !name.Contains("boo.") &&
                                                !name.Contains("serializ") && !name.Contains("json") &&
                                                !name.Contains("log.") && !name.Contains("logging") &&
                                                !name.Contains("test") && !name.Contains("editor") &&
                                                !name.Contains("debug") && !name.Contains("ms.internal") &&
                                                !name.Contains("privateimplementation") && !name.Contains("jetbrains") &&
                                                !name.Contains("firebase") && !name.Contains("excss") &&
                                                !name.Contains("<") && !name.Contains("google") &&
                                                !name.Contains("tmpro") && !name.Contains("cdi.") &&
                                                !name.Contains("dg.") && !name.Contains("facebookgames") && !name.Contains("+"));

        return AllTypes = res.Where(i => x(i.ToString().ToLower())).ToList();
    }

    public static IEnumerable<FieldInfo> GetSerializedFieldInfos<TFieldType>(this Type type)
    {
        return type.GetFields(CommonBindings)
            .Where(i => (i.IsPublic || i.GetCustomAttribute<SerializeField>() != null)
                        && (typeof(TFieldType).IsAssignableFrom(i.FieldType)
                        || i.FieldType.IsArray && typeof(TFieldType).IsAssignableFrom(i.FieldType.GetElementType())));
    }

    public static IEnumerable<TFieldType> GetSerializedFields<TFieldType>(this Object target) where TFieldType : class
    {
        List<TFieldType> GetValues(FieldInfo fieldInfo)
        {
            var rets = new List<TFieldType>();

            var v = fieldInfo.GetValue(target);
            if (!fieldInfo.FieldType.IsArray)
            {
                rets.Add(v as TFieldType);
            }
            else if (v is IEnumerable it)
            {
                rets.AddRange(it.OfType<TFieldType>());
            }

            return rets;
        }

        return GetSerializedFieldInfos<TFieldType>(target.GetType())
            .SelectMany(GetValues)
            .Where(i => i != null);
    }
}
