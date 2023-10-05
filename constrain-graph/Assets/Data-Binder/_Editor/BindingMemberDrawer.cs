#if UNITY_EDITOR
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shahant.DataBinder
{
    public partial class BindingMember
    {

        [CustomPropertyDrawer(typeof(BindingMember))]
        public class BindingMemberDrawer : PropertyDrawer
        {
            internal const string kTargetString = "Target";
            internal const string kMemberString = "Member";
            internal const string kNoMemberString = "No Member";

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                
                EditorGUI.BeginProperty(position, label, property);
                EditorGUI.BeginChangeCheck();
                float width = position.width - position.x;
                var targetRect = new Rect(position.x, position.y, width * 0.5f - 3, position.height);
                var memberRect = new Rect(targetRect.xMax + 3, position.y, position.width - targetRect.width, position.height);
                memberRect.xMax = position.xMax;
                var labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 50;
                
                var targetProp = property.FindPropertyRelative(kTargetString);
                var memberProp = property.FindPropertyRelative(kMemberString);
                EditorGUI.PropertyField(targetRect, targetProp, new GUIContent(kTargetString));

                DrawMember(memberRect, targetProp, memberProp);

                EditorGUIUtility.labelWidth = labelWidth;
                if (EditorGUI.EndChangeCheck())
                {

                    property.serializedObject.ApplyModifiedProperties();
                    property.serializedObject.Update();
                }

                EditorGUI.EndProperty();
            }

            private void DrawMember(Rect rect, 
                SerializedProperty targetProp, 
                SerializedProperty memberProp)
            {
                using (new EditorGUI.DisabledScope(targetProp.objectReferenceValue == null))
                {

                    if (GUI.Button(rect, new GUIContent(GetButtonLabel(targetProp, memberProp)), EditorStyles.popup))
                    {
                        GenericMenu menu = GetMethodGenericMenu(targetProp, memberProp);
                        menu.DropDown(rect);
                    }
                }
            }

            private string GetButtonLabel(SerializedProperty targetProp, SerializedProperty memberProp)
            {
                if (targetProp == null) return kNoMemberString;
                if (targetProp.objectReferenceValue == null) return kNoMemberString; 
                if (memberProp == null) return kNoMemberString;
                if( string.IsNullOrEmpty(memberProp.stringValue)) return kNoMemberString;

                var targetType = targetProp.objectReferenceValue.GetType();
                var memberInfo = targetProp.objectReferenceValue.GetType().GetMember(memberProp.stringValue);
                if(memberInfo.Length == 0)
                {
                    return $"<Missing {targetType.Name}.{memberProp.stringValue}>";
                }
                else
                {
                    if (memberProp.stringValue.StartsWith("set_"))
                    {
                        return $"{targetType.Name}.{memberProp.stringValue.Substring(4)}";
                    }
                    else return $"{targetType.Name}.{memberProp.stringValue}";
                }

            }
            
            private GenericMenu GetMethodGenericMenu(SerializedProperty targetProp, SerializedProperty memberProp)
            {
                Object target = targetProp.objectReferenceValue;
                if(target is Component)
                {
                    target = (target as Component).gameObject;
                }
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent(kNoMemberString), string.IsNullOrEmpty(memberProp.stringValue), () => SetNoMember(targetProp, memberProp));
                if (target == null) return menu;
                menu.AddSeparator(string.Empty);

                if (target is GameObject)
                {
                    Component[] components = (target as GameObject).GetComponents<Component>();
                    for(int i = 0; i < components.Length; ++i)
                    {
                        var component = components[i];
                        if (component == null) continue;
                        var type = component.GetType();

                        var properties = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                        foreach(var _ in properties)
                        {
                            menu.AddItem(new GUIContent($"{type.Name}/{_.Name} ({_.PropertyType.Name})"), false, () => SetMember(targetProp, component, memberProp, _.Name));
                        }
                        menu.AddSeparator($"{type.Name}/");
                        var methods = component.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
                        foreach(var _ in methods)
                        {
                            menu.AddItem(new GUIContent($"{type.Name}/{_.Name} ({_.ReturnType.Name})"), false, () => SetMember(targetProp, component, memberProp, _.Name));
                        }
                    }
                    
                }
               
                return menu;
            }

            private void SetNoMember(SerializedProperty targetProp, SerializedProperty memberProp)
            {
                
                if(targetProp.objectReferenceValue is Component)
                {
                    targetProp.objectReferenceValue = (targetProp.objectReferenceValue as Component).gameObject;
                    targetProp.serializedObject.ApplyModifiedProperties();
                    targetProp.serializedObject.Update();
                }
                memberProp.stringValue = string.Empty;
                memberProp.serializedObject.ApplyModifiedProperties();
                memberProp.serializedObject.Update();
            }

            private void SetMember(SerializedProperty targetProp, Object targetValue, SerializedProperty memberProp, string memberStr)
            {
                targetProp.objectReferenceValue = targetValue;
                targetProp.serializedObject.ApplyModifiedProperties();
                targetProp.serializedObject.Update();

                memberProp.stringValue = memberStr;
                memberProp.serializedObject.ApplyModifiedProperties();
                memberProp.serializedObject.Update();
                
            }
        }
    }
}
#endif
