using MyCustomEditor;
using UnityEditor;
using UnityEngine;

namespace MyCustomEditor
{
    [CustomEditor(typeof(Effect))]
    public class EffectEditor : BaseObjectEditor
    {
        private SerializedProperty _typeProperty;
        private SerializedProperty _isAllowDuplicateProperty;
        private SerializedProperty _removeDuplicateTargetOptionProperty;
        private SerializedProperty _isShowInUIProperty;
        private SerializedProperty _isAllowLevelExceedDataProperty;
        private SerializedProperty _maxLevelProperty;
        private SerializedProperty _effectDataProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            _typeProperty = serializedObject.FindProperty("type");
            _isAllowDuplicateProperty = serializedObject.FindProperty("isAllowDuplicate");
            _removeDuplicateTargetOptionProperty = serializedObject.FindProperty("removeDuplicateTargetOption");
            _isShowInUIProperty = serializedObject.FindProperty("isShowInUI");
            _isAllowLevelExceedDataProperty = serializedObject.FindProperty("isAllowLevelExceedData");
            _maxLevelProperty = serializedObject.FindProperty("maxLevel");
            _effectDataProperty = serializedObject.FindProperty("effectData");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();

            float prevLevelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 200f;
            
            DrawSettings();
            DrawOptions();
            DrawEffectData();

            EditorGUIUtility.labelWidth = prevLevelWidth;

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSettings()
        {
            if (!DrawFoldoutTitle("설정"))
                return;

            CustomEditorUtility.DrawEnumToolbar(_typeProperty);

            EditorGUILayout.Space();
            CustomEditorUtility.DrawUnderline();
            EditorGUILayout.Space();

            _isAllowDuplicateProperty.DrawPropertyField();

            if (!_isAllowDuplicateProperty.boolValue) 
                CustomEditorUtility.DrawEnumToolbar(_removeDuplicateTargetOptionProperty);
        }

        private void DrawOptions()
        {
            if (!DrawFoldoutTitle("옵션"))
                return;

            _isShowInUIProperty.DrawPropertyField();
        }

        private void DrawEffectData()
        {
            if (_effectDataProperty.arraySize == 0)
            {
                _effectDataProperty.arraySize++;
                _effectDataProperty.GetArrayElementAtIndex(0).FindPropertyRelative("level").intValue = 1;
            }
            
            if (!DrawFoldoutTitle("데이터"))
                return;

            _isAllowLevelExceedDataProperty.DrawPropertyField();

            if (_isAllowLevelExceedDataProperty.boolValue)
            {
                _maxLevelProperty.DrawPropertyField();
            }
            else
            {
                GUI.enabled = false;

                var lastEffectData = _effectDataProperty.GetArrayElementAtIndex(_effectDataProperty.arraySize - 1);
                
                _maxLevelProperty.intValue = lastEffectData.FindPropertyRelative("level").intValue;
                _maxLevelProperty.DrawPropertyField();

                GUI.enabled = true;
            }

            for (int i = 0; i < _effectDataProperty.arraySize; i++)
            {
                var property = _effectDataProperty.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginVertical("HelpBox");
                {
                    if (DrawRemovableLevelFoldout(_effectDataProperty, property, i, i != 0))
                    {
                        EditorGUILayout.EndVertical();
                        break;
                    }

                    if (property.isExpanded)
                    {
                        EditorGUI.indentLevel += 1;

                        var levelProperty = property.FindPropertyRelative("level");
                        DrawAutoSortLevelProperty(_effectDataProperty, levelProperty, i, i != 0);

                        var maxStackProperty = property.FindPropertyRelative("maxStack");
                        maxStackProperty.DrawPropertyField();
                        maxStackProperty.intValue = Mathf.Max(maxStackProperty.intValue, 1);

                        var stackActionsProperty = property.FindPropertyRelative("stackActions");
                        var prevStackActionSize = stackActionsProperty.arraySize;

                        stackActionsProperty.DrawPropertyField();

                        if (stackActionsProperty.arraySize > prevStackActionSize)
                        {
                            var lastStackActionProperty = stackActionsProperty.GetArrayElementAtIndex(prevStackActionSize);
                            var actionProperty = lastStackActionProperty.FindPropertyRelative("action");

                            CustomEditorUtility.DeepCopySerializeReference(actionProperty);
                        }

                        for (int stackActionIndex = 0; stackActionIndex < stackActionsProperty.arraySize; stackActionIndex++)
                        {
                            var stackActionProperty = stackActionsProperty.GetArrayElementAtIndex(stackActionIndex);
                            var stackProperty = stackActionProperty.FindPropertyRelative("stack");
                            stackProperty.intValue = Mathf.Clamp(stackProperty.intValue, 1, maxStackProperty.intValue);
                        }

                        property.FindPropertyRelative("action").DrawPropertyField();
                        property.FindPropertyRelative("runningFinishOption").DrawPropertyField();
                        property.FindPropertyRelative("duration").DrawPropertyField();
                        property.FindPropertyRelative("applyCount").DrawPropertyField();
                        property.FindPropertyRelative("applyCycle").DrawPropertyField();
                        property.FindPropertyRelative("customActions").DrawPropertyField();

                        EditorGUI.indentLevel -= 1;
                    }
                }
                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Add New Level"))
            {
                var lastArraySize = _effectDataProperty.arraySize++;
                var prevElementProperty = _effectDataProperty.GetArrayElementAtIndex(lastArraySize - 1);
                var newElementProperty = _effectDataProperty.GetArrayElementAtIndex(lastArraySize);
                var newElementLevel = prevElementProperty.FindPropertyRelative("level").intValue + 1;
                newElementProperty.FindPropertyRelative("level").intValue = newElementLevel;

                CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("stackActions"), "action");
                CustomEditorUtility.DeepCopySerializeReference(newElementProperty.FindPropertyRelative("action"));
                CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("customActions"));
            }
        }
    }
}