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
            internal const string kTargetString = "Target";
            internal const string kMemberString = "Member";
            internal const string kNoMemberString = "No Member";

            private List<string> classNames = new List<string>() { kNoMemberString, "item1", "item2" };

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
                    property.serializedObject.ApplyModifiedProperties();
                EditorGUI.EndProperty();
            }

            private void DrawMember(Rect rect, SerializedProperty targetProp, SerializedProperty memberProp)
            {
                if (GUI.Button(rect, new GUIContent(GetMemberString(targetProp, memberProp)), EditorStyles.popup))
                {
                    //var popup = BuildPopupList.Invoke(null, new object[] { listenerTarget.objectReferenceValue, m_DummyEvent, pListener }) as GenericMenu;
                    //popup.DropDown(functionRect);
                }
            }

            private string GetMemberString(SerializedProperty targetProp, SerializedProperty memberProp)
            {
                if (targetProp == null) return kNoMemberString;
                if (targetProp.objectReferenceValue == null) return kNoMemberString; 
                if (memberProp == null) return kNoMemberString;
                if( memberProp.objectReferenceValue == null) return kNoMemberString;


                string result = memberProp.objectReferenceValue.ToString();
                // check missing member in targerProp
                return $"{memberProp.objectReferenceValue.ToString()}";
            }
            
        }
#endif
    }


}
