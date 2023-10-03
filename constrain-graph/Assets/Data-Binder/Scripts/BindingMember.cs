using System.Reflection;

using UnityEngine;
using System.Collections.Generic;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace Shahant.DataBinder
{
    [System.Serializable]
    public class BindingMember
    {
        public Object Target;
        public string Member;

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(BindingMember))]
        public class MemberDrawer : PropertyDrawer
        {
            internal const string kNoFunctionString = "No Function";
            
            private List<string> classNames = new();

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
                var targetProperty = property.FindPropertyRelative("Target");
                EditorGUI.PropertyField(targetRect, targetProperty, new GUIContent("Target"));

                EditorGUIUtility.labelWidth = 0;
                EditorGUI.PropertyField(memberRect, property.FindPropertyRelative("Member"), GUIContent.none);
                EditorGUIUtility.labelWidth = labelWidth;
                if (EditorGUI.EndChangeCheck())
                    property.serializedObject.ApplyModifiedProperties();
                EditorGUI.EndProperty();
            }

            private void CreateListView(SerializedProperty property)
            {

            }

            
        }
#endif
    }


}
