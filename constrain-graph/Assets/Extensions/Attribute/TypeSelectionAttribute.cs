using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Shahant
{
    public class TypeSelectionAttribute : PropertyAttribute
    {
        public readonly Type type;
        public float labelWidth = -1;
        public bool showAsError;

        public TypeSelectionAttribute(Type type)
        {
            this.type = type;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(TypeSelectionAttribute))]
    public class _Drawer : PropertyDrawer
    {
        const string kSearchStr = "...";
        const float btnWidth = 25;

        private bool IsValid(SerializedProperty property) =>
            (attribute as TypeSelectionAttribute).type.IsInstanceOfType(property.objectReferenceValue);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var att = attribute as TypeSelectionAttribute;

            var valid = IsValid(property);
            if (!valid)
            {
                label.tooltip = $"{ObjectNames.NicifyVariableName(property.name)} must be of type {att?.type}";
            }

            using (new LabelWidth(att.labelWidth > 0 ? att.labelWidth : EditorGUIUtility.labelWidth))
            using (new GuiColor(valid ? Color.white : att.showAsError ? Color.red : Color.yellow))
            {
                position.width -= btnWidth + 2;
                EditorGUI.ObjectField(position, property, label);
            }

            var btnRect = position;
            btnRect.x += position.width + 2;
            btnRect.width = btnWidth;
            Menu(btnRect, property, att.type);

            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        private static void Menu(Rect rect, SerializedProperty property, Type type)
        {
            var value = property.objectReferenceValue ? property.objectReferenceValue : property.serializedObject.targetObject;

            var go = value as GameObject ?? (value as Component)?.gameObject;

            if (go != null)
            {
                using (new GuiColor(Color.cyan))
                {
                    if (!GUI.Button(rect, kSearchStr)) return;
                }

                var candidates = go.GetComponentsInChildren(type, true);
                var menu = new GenericMenu();
                menu.AddDisabledItem(new GUIContent($"{type.Name}"));

                for (int i = 0; i < candidates.Length; i++)
                {
                    Component c = candidates[i];
                    menu.AddItem(new GUIContent($"({i}) {c.GetType().Name}"), value == c,
                        () =>
                        {
                            property.objectReferenceValue = c;
                            property.serializedObject.ApplyModifiedProperties();
                        });
                }

                menu.ShowAsContext();
            }
            else
            {
                using (new DisabledGui(true))
                {
                    GUI.Button(rect, kSearchStr);
                }
            }


        }
    }
#endif
}

