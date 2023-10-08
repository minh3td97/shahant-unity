#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Shahant.DataBinder
{
    [CustomEditor(typeof(DataBinder))]
    public class DataBinder_Editor : Editor
    {
        internal const string kProviderNameStr = "_provider";
        internal const string kTargetsNameStr = "_targets";

        protected SerializedProperty _providerProp;
        protected SerializedProperty _bindingTargetsProp;
        protected ReorderableList _bindingTargetsRL;

        DataBinder _dataBinder;

        float padding = 5;
        float margin = 5;
        float spacing = 2;

        public void OnEnable()
        {
            _dataBinder = target as DataBinder;
            _providerProp = serializedObject.FindProperty(kProviderNameStr);
            _bindingTargetsProp = serializedObject.FindProperty(kTargetsNameStr);

            _bindingTargetsRL = new ReorderableList(serializedObject, _bindingTargetsProp, true, true, true, true);
            _bindingTargetsRL.drawElementCallback = BindingTargetsRL_DrawElement;
            _bindingTargetsRL.drawHeaderCallback = BindingTargetsRL_DrawHeader;
            _bindingTargetsRL.elementHeightCallback = BindingTargetsRL_ElementHeight;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            OnInspectorGUI_DataBinder();
            serializedObject.ApplyModifiedProperties();
        }

        public virtual void OnInspectorGUI_DataBinder()
        {
            
            EditorGUILayout.PropertyField(_providerProp, new GUIContent("Provider"));
            using(new EditorGUI.DisabledScope(_dataBinder.DataProvider == null))
            {
                EditorGUILayout.Space();
                DrawFilter();
                _bindingTargetsRL.DoLayoutList();
            }
            
        }

        private void DrawFilter()
        {
            if (_dataBinder.DataProvider != null)
            {
                var provider = _dataBinder.DataProvider;
                var flag = _dataBinder.Flag;
                _dataBinder.Flag = DrawBindingFlag(provider.BindingFilters, flag);
                if (_dataBinder.Flag != flag)
                    EditorUtility.SetDirty(_dataBinder);
            }
        }

        private bool flagFoldout;
        private int DrawBindingFlag(string[] options, int value)
        {
            void Draw()
            {
                EditorGUI.indentLevel++;

                for (var i = 0; i < options.Length; i++)
                {
                    if (i % 2 == 0) EditorGUILayout.BeginHorizontal();

                    var selected = ((1 << i) & value) != 0;
                    var style = new GUIStyle(GUI.skin.toggle)
                    {
                        normal = { textColor = selected ? Color.cyan : Color.white }
                    };
                    if (EditorGUILayout.Toggle(options[i], selected, style))
                    {
                        value |= 1 << i;
                    }
                    else
                    {
                        value &= ~(1 << i);
                    }

                    if (i % 2 == 1 || i == options.Length - 1) EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
            }

            using (new Indent())
            using (new VerticalHelpBox())
            {
                using (new HorizontalLayout())
                {
                    flagFoldout = EditorGUILayout.Foldout(flagFoldout, $"Filter ({value})", true);

                    using (new GuiColor(Color.cyan))
                        if (GUILayout.Button("ALL", GUILayout.Width(60)))
                        {
                            value = All(options);
                        }

                    using (new GuiColor(Color.cyan))
                        if (GUILayout.Button("NONE", GUILayout.Width(60)))
                        {
                            value = 0x00;
                        }

                }

                if (flagFoldout) Draw();
            }


            return value;
        }

        private int All(string[] options)
        {
            var s = 0;
            for (var i = 0; i < options.Length; i++) s |= 1 << i;
            return s;
        }

        protected void BindingTargetsRL_DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty property = _bindingTargetsRL.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.BeginProperty(rect, GUIContent.none, property);
            EditorGUI.BeginChangeCheck();
            Rect boxRect = new Rect(rect.x, rect.y + padding, rect.width, rect.height - 2 * padding);
            
            GUI.Box(boxRect, GUIContent.none, EditorStyles.helpBox);
            Rect contentRect = new Rect(boxRect.x + margin, boxRect.y + margin, boxRect.width - 2 * margin, boxRect.height - 2 * margin);
            float height = (contentRect.height - 2 * spacing) / 3;
            Rect valueRect = new Rect(contentRect.x, contentRect.y, contentRect.width, height);
            Rect targetRect = new Rect(contentRect.x, valueRect.yMax + spacing, contentRect.width, height);
            Rect converterRect = new Rect(contentRect.x, targetRect.yMax + spacing, contentRect.width, height);
            
            BindingTarget_Drawer.DrawValueProperty(valueRect, _providerProp.objectReferenceValue, property.FindPropertyRelative("Value"), new GUIContent("Value"));
            EditorGUI.PropertyField(targetRect, property.FindPropertyRelative("Target"), new GUIContent("Target"));
            EditorGUI.PropertyField(converterRect, property.FindPropertyRelative("Converter"), new GUIContent("Converter"));

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
            }
        }

        protected float BindingTargetsRL_ElementHeight(int index)
        {
            return 60 + 2 *(padding + margin);
        }

        protected void BindingTargetsRL_DrawHeader(Rect rect)
        {
            string name = "Targets";
            EditorGUI.LabelField(rect, name);
        }

        
    }
}
#endif