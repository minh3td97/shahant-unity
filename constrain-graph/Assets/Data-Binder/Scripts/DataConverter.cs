using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Shahant
{

    [Serializable]
    public class DataConverter
    {
        public Type type;
        public string format = "{0}";
        public double defaultValue;

        public object Convert(object data)
        {
            return Convert(data, type);
        }

        private object Convert(object data, Type dtype)
        {
            switch (dtype)
            {
                case Type.None:
                    return data;

                case Type.String:
                    return string.Format(format, data);

                case Type.LowerCaseString:
                    return string.Format(format, data).ToLower();

                case Type.UpperCaseString:
                    return string.Format(format, data).ToUpper();

                case Type.CapitalizedString:
                    return string.Format(format, data);
                
                case Type.Int:
                {
                    if (data?.GetType() == typeof(int)) return data;
                    var str = $"{data}";
                    return int.TryParse(str, out int v) ? v : (int) defaultValue;
                }

                case Type.Long:
                {
                    if (data?.GetType() == typeof(long)) return data;
                    var str = $"{data}";
                    return long.TryParse(str, out long v) ? v : (long) defaultValue;
                }

                case Type.Float:
                {
                    if (data?.GetType() == typeof(float)) return data;
                    var str = $"{data}";
                    return float.TryParse(str, out float v) ? v : (float) defaultValue;
                }

                case Type.Double:
                {
                    if (data?.GetType() == typeof(double)) return data;
                    var str = $"{data}";
                    return double.TryParse(str, out double v) ? v : defaultValue;
                }

                case Type.Bool:
                {
                    var type = data?.GetType();
                    if (type == typeof(bool)) return data;
                    if (IsNumericType(type)) return (int) data != 0;

                    if (type != typeof(string)) return data != null;
                    var str = $"{data}";
                    return int.TryParse(str, out int v) ? (v != 0) : (int) defaultValue != 0;

                }

                case Type.NotBool:
                {
                    return !(bool) Convert(data, Type.Bool);
                }

                default: return data;
            }

        }

        public bool IsNumericType(System.Type type)
        {
            if (type == null) return false;
            switch (System.Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public DataConverter Clone()
        {
            return new DataConverter
            {
                type = type,
                format = (string) format.Clone(),
                defaultValue = defaultValue
            };
        }

        public enum Type
        {
            None,
            String,
            Int,
            Long,
            Float,
            Double,
            Bool,
            NotBool,
            LowerCaseString,
            UpperCaseString,
            CapitalizedString
        }
    }



#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DataConverter))]
    public class ConverterDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            float lw = EditorGUIUtility.labelWidth;
            Rect typeRect = new Rect(position.x, position.y, lw - 2, position.height);
            Rect formatRect = new Rect(position.x + lw, position.y, position.width - lw, position.height);
            
            EditorGUIUtility.labelWidth = 50;
            var typeProperty = property.FindPropertyRelative("type");
            EditorGUI.PropertyField(typeRect, typeProperty, new GUIContent("Convert"));

            position.x += position.width;
            int type = property.FindPropertyRelative("type").intValue;
            if (type == (int) DataConverter.Type.String
                || type == (int) DataConverter.Type.LowerCaseString
                || type == (int) DataConverter.Type.UpperCaseString
                || type == (int) DataConverter.Type.CapitalizedString
            )
            {
                EditorGUI.PropertyField(formatRect, property.FindPropertyRelative("format"), new GUIContent("Format"));
            }
            else if (type != 0)
            {
                EditorGUI.PropertyField(formatRect, property.FindPropertyRelative("defaultValue"),
                    new GUIContent("Default"));
            }

            EditorGUIUtility.labelWidth = lw;
            EditorGUI.EndProperty();
        }
    }
#endif

}