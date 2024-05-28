using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MyCustomEditor
{
    public static class CustomEditorUtility
    {
        private static readonly GUIStyle TitleStyle;

        static CustomEditorUtility()
        {
            TitleStyle = new GUIStyle("ShurikenModuleTitle")
            {
                font = new GUIStyle(EditorStyles.label).font,
                fontStyle = FontStyle.Bold,
                fontSize = 14,
                border = new RectOffset(15, 7, 4, 4),
                fixedHeight = 26f,
                contentOffset = new Vector2(20f, -2f)
            };
        }

        public static bool DrawFoldoutTitle(string title, bool isExpanded, float space = 15f)
        {
            EditorGUILayout.Space(space);

            var rect = GUILayoutUtility.GetRect(16f, TitleStyle.fixedHeight, TitleStyle);

            GUI.Box(rect, title, TitleStyle);

            var currentEvent = Event.current;
            var toggleRect = new Rect(rect.x + 4f, rect.y + 4f, 13f, 13f);

            if (currentEvent.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(toggleRect, false, false, isExpanded, false);
            }
            else if (currentEvent.type == EventType.MouseDown && rect.Contains(currentEvent.mousePosition))
            {
                isExpanded = !isExpanded;
                currentEvent.Use();
            }

            return isExpanded;
        }

        public static bool DrawFoldoutTitle(IDictionary<string, bool> isFoldoutExpandedByTitle, string title, float space = 15f)
        {
            isFoldoutExpandedByTitle.TryAdd(title, true);
            isFoldoutExpandedByTitle[title] = DrawFoldoutTitle(title, isFoldoutExpandedByTitle[title], space);

            return isFoldoutExpandedByTitle[title];
        }

        public static void DrawUnderline(float height = 1f)
        {
            var lastRect = GUILayoutUtility.GetLastRect();

            lastRect = EditorGUI.IndentedRect(lastRect);
            lastRect.y += lastRect.height;
            lastRect.height = height;

            EditorGUI.DrawRect(lastRect, Color.gray);
        }

        public static void DrawEnumToolbar(SerializedProperty enumProperty)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(enumProperty.displayName);
            enumProperty.enumValueIndex = GUILayout.Toolbar(enumProperty.enumValueIndex, enumProperty.enumDisplayNames);
            EditorGUILayout.EndHorizontal();
        }

        public static void DeepCopySerializeReference(SerializedProperty property)
        {
            if (property.managedReferenceValue == null)
                return;

            property.managedReferenceValue = (property.managedReferenceValue as ICloneable).Clone();
        }

        public static void DeepCopySerializeReferenceArray(SerializedProperty property, string fieldName = "")
        {
            for (int i = 0; i < property.arraySize; i++)
            {
                var elementProperty = property.GetArrayElementAtIndex(i);

                if (!string.IsNullOrEmpty(fieldName)) 
                    elementProperty = elementProperty.FindPropertyRelative(fieldName);

                elementProperty.managedReferenceValue = (elementProperty.managedReferenceValue as ICloneable).Clone();
            }
        }
        
        // DisplayName과 Tooltip 추가/수정을 쉽게하기 위한 확장 메소드
        public static void DrawPropertyField(this SerializedProperty property, string label = null, string tooltip = null, params GUILayoutOption[] options)
        {
            label ??= property.displayName;
            tooltip ??= property.tooltip;
            
            EditorGUILayout.PropertyField(property, new GUIContent(label, tooltip), options);
        }
    }
}