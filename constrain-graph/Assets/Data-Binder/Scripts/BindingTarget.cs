using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

namespace Shahant.DataBinder
{
    [System.Serializable]
    public class BindingTarget
    {
        public string Value;
        public BindingMember Target;
        public DataConverter Converter;

        private Wrapper wrapper;

        public void Init(Type type, UnityEngine.Object context)
        {
            wrapper = new Wrapper();
            Target.Setup();
            wrapper.Init(type, Value, context);
        }

        public void Bind(object data)
        {
            data = wrapper.GetValue(data);
            Target.SetValue(Converter.Convert(data));
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(BindingTarget))]
    public class BindingTarget_Drawer : PropertyDrawer
    {
        internal const string kNoValueStr = "No Value";
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            float height = position.height / 3 - 1;
            Rect valueRect = new Rect(position.x, position.y, position.width, height);
            Rect targetRect = new Rect(position.x, valueRect.yMax + 2, position.width, height);
            Rect converterRect = new Rect(position.x, targetRect.yMax + 2, position.width, height);
            converterRect.yMax = position.yMax;
            
            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("Value"), new GUIContent("Value"));
            EditorGUI.PropertyField(targetRect, property.FindPropertyRelative("Target"), new GUIContent("Target"));
            EditorGUI.PropertyField(converterRect, property.FindPropertyRelative("Converter"), new GUIContent("Converter"));

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            
            float height = 0;
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Value"), GUIContent.none);
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Target"), GUIContent.none);
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Converter"), GUIContent.none);
            height += 4;
            return height;
        }

        public static void DrawValueProperty(Rect position, UnityEngine.Object obj, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            float width = EditorGUIUtility.labelWidth;
            Rect labelRect = new Rect(position.x, position.y, width, position.height);
            GUI.Label(labelRect, label);
            Rect buttonRect = new Rect(position.x + width, position.y, position.width - width, position.height);
            if (GUI.Button(buttonRect, GetValueStringName(obj, property), EditorStyles.popup))
            {
                GenericMenu menu = GetFieldsOrProperties(obj, property);
                menu.DropDown(buttonRect);
            }
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
            }
        }

        static GenericMenu GetFieldsOrProperties(UnityEngine.Object obj, SerializedProperty property)
        {

            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent(kNoValueStr), false, () =>
            {
                property.stringValue = string.Empty;
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
            });
            if (obj == null) return menu;

            var fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            if(fields.Length > 0)
            {
                menu.AddSeparator(string.Empty);
                foreach (var _ in fields)
                    menu.AddItem(new GUIContent($"{_.Name} ({_.FieldType.Name})"), false, () => 
                    {
                        property.stringValue = _.Name;
                        property.serializedObject.ApplyModifiedProperties();
                        property.serializedObject.Update();
                    });
            }

            var properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if(properties.Length > 0)
            {
                menu.AddSeparator(string.Empty);
                foreach(var _ in properties)
                {
                    menu.AddItem(new GUIContent($"{_.Name} ({_.PropertyType.Name})"), false, () =>
                    {
                        property.stringValue = _.Name;
                        property.serializedObject.ApplyModifiedProperties();
                        property.serializedObject.Update();
                    });
                }
            }
            return menu;
            
        }

        static string GetValueStringName(UnityEngine.Object obj, SerializedProperty property)
        {
            if (obj == null) return kNoValueStr;
            if (string.IsNullOrEmpty(property.stringValue)) return kNoValueStr;

            var targetType = obj.GetType();
            var memberInfo = targetType.GetMember(property.stringValue);
            if (memberInfo.Length == 0)
            {
                return $"<Missing {targetType.Name}.{property.stringValue}>";
            }
            else
            {
                if (property.stringValue.StartsWith("set_"))
                {
                    return $"{property.stringValue.Substring(4)}";
                }
                else return $"{property.stringValue}";
            }
        }
    }
#endif
}
