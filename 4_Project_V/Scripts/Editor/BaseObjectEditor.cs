using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MyCustomEditor
{
    [CustomEditor(typeof(BaseObject), true)]
    public class BaseObjectEditor : Editor
    {
        private SerializedProperty _categoriesProperty;
        private SerializedProperty _idProperty;
        private SerializedProperty _iconProperty;
        private SerializedProperty _codeNameProperty;
        private SerializedProperty _displayNameProperty;
        private SerializedProperty _descriptionProperty;

        // Inspector 상에서 순서를 편집할 수 있는 List
        private ReorderableList _categories;

        // text를 넓게 보여주는 Style(=Skin) 지정을 위한 변수
        private GUIStyle _textAreaStyle;

        // Title의 Foldout Expand 상태를 저장하는 변수
        private readonly Dictionary<string, bool> _isFoldoutExpandedByTitle = new();

        protected virtual void OnEnable()
        {
            GUIUtility.keyboardControl = 0;
            
            _categoriesProperty = serializedObject.FindProperty("categories");
            _idProperty = serializedObject.FindProperty("id");
            _iconProperty = serializedObject.FindProperty("icon");
            _codeNameProperty = serializedObject.FindProperty("codeName");
            _displayNameProperty = serializedObject.FindProperty("displayName");
            _descriptionProperty = serializedObject.FindProperty("description");


            _categories = new ReorderableList(serializedObject, _categoriesProperty);
            _categories.drawHeaderCallback = rect => EditorGUI.LabelField(rect, _categoriesProperty.displayName);
            _categories.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect = new Rect(rect.x, rect.y + 2f, rect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(rect, _categoriesProperty.GetArrayElementAtIndex(index), GUIContent.none);
            };
        }

        private void StyleInit()
        {
            if (_textAreaStyle == null)
            {
                _textAreaStyle = new GUIStyle(EditorStyles.textArea);
                _textAreaStyle.wordWrap = true;
            }
        }

        protected bool DrawFoldoutTitle(string text)
            => CustomEditorUtility.DrawFoldoutTitle(_isFoldoutExpandedByTitle, text);

        public override void OnInspectorGUI()
        {
            StyleInit();
            serializedObject.Update();
            
            _categories.DoLayoutList();

            if (DrawFoldoutTitle("기본 정보"))
            {
                EditorGUILayout.BeginHorizontal("HelpBox");
                {
                    _iconProperty.objectReferenceValue = EditorGUILayout.ObjectField(GUIContent.none,
                        _iconProperty.objectReferenceValue, typeof(Sprite), false, GUILayout.Width(65));

                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUI.enabled = false;
                            EditorGUILayout.PrefixLabel("ID");
                            EditorGUILayout.PropertyField(_idProperty, GUIContent.none);
                            GUI.enabled = true;
                        }
                        EditorGUILayout.EndHorizontal();
                        
                        EditorGUI.BeginChangeCheck();

                        var prevCodeName = _codeNameProperty.stringValue;
                        EditorGUILayout.DelayedTextField(_codeNameProperty);

                        if (EditorGUI.EndChangeCheck())
                        {
                            var assetPath = AssetDatabase.GetAssetPath(target);
                            var newName = $"{target.GetType().Name.ToUpper()}_{_codeNameProperty.stringValue}";

                            serializedObject.ApplyModifiedProperties();

                            var message = AssetDatabase.RenameAsset(assetPath, newName);

                            if (string.IsNullOrEmpty(message))
                                target.name = newName;
                            else
                                _codeNameProperty.stringValue = prevCodeName;
                        }

                        EditorGUILayout.PropertyField(_displayNameProperty);
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical("HelpBox");
                {
                    EditorGUILayout.LabelField("Description");
                    _descriptionProperty.stringValue = EditorGUILayout.TextArea(_descriptionProperty.stringValue,
                        _textAreaStyle, GUILayout.Height(60));
                }
                EditorGUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected bool DrawRemovableLevelFoldout(SerializedProperty dataProperty, SerializedProperty targetProperty,
            int targetIndex, bool isDrawRemoveButton)
        {
            bool isRemoveButtonClicked = false;

            EditorGUILayout.BeginHorizontal();
            {
                GUI.color = Color.yellow;
                var level = targetProperty.FindPropertyRelative("level").intValue;

                targetProperty.isExpanded = EditorGUILayout.Foldout(targetProperty.isExpanded, $"Level {level}");
                GUI.color = Color.white;

                if (isDrawRemoveButton)
                {
                    GUI.color = Color.red;

                    if (GUILayout.Button("\u00d7", EditorStyles.miniButton, GUILayout.Width(20)))
                    {
                        isRemoveButtonClicked = true;
                        dataProperty.DeleteArrayElementAtIndex(targetIndex);
                    }
                    GUI.color = Color.white;
                }
            }
            EditorGUILayout.EndHorizontal();

            return isRemoveButtonClicked;
        }

        protected void DrawAutoSortLevelProperty(SerializedProperty dataProperty, SerializedProperty levelProperty,
            int index, bool isEditable)
        {
            if (!isEditable)
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(levelProperty);
                GUI.enabled = true;
            }
            else
            {
                EditorGUI.BeginChangeCheck();

                var prevValue = levelProperty.intValue;

                EditorGUILayout.DelayedIntField(levelProperty);

                if (EditorGUI.EndChangeCheck())
                {
                    if (levelProperty.intValue <= 1)
                    {
                        levelProperty.intValue = prevValue;
                    }
                    else
                    {
                        for (int i = 0; i < dataProperty.arraySize; i++)
                        {
                            if (index == i)
                                continue;

                            var element = dataProperty.GetArrayElementAtIndex(i);

                            if (element.FindPropertyRelative("level").intValue == levelProperty.intValue)
                            {
                                levelProperty.intValue = prevValue;
                                break;
                            }
                        }

                        if (levelProperty.intValue != prevValue)
                        {
                            for (int moveIndex = 1; moveIndex < dataProperty.arraySize; moveIndex++)
                            {
                                if (moveIndex == index)
                                    continue;

                                var element = dataProperty.GetArrayElementAtIndex(moveIndex).FindPropertyRelative("level");

                                if (levelProperty.intValue < element.intValue || moveIndex == dataProperty.arraySize - 1)
                                {
                                    dataProperty.MoveArrayElement(index, moveIndex);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}