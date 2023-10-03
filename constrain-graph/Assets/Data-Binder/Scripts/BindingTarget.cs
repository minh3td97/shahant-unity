#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace Shahant.DataBinder
{
    [System.Serializable]
    public class BindingTarget
    {
        public string Value;
        public BindingMember Target;
        public string Converter;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(BindingTarget))]
    public class BindingTarget_Drawer : PropertyDrawer
    {
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
            /// Height of the label
            //float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            float height = 0;
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Value"), GUIContent.none);
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Target"), GUIContent.none);
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Converter"), GUIContent.none);
            height += 4;
            return height;
        }
    }
#endif
}
