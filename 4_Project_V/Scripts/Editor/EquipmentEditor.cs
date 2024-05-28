using UnityEditor;
using UnityEngine;

namespace MyCustomEditor
{
    [CustomEditor(typeof(Equipment))]
    public class EquipmentEditor : ItemEditor
    {
        private SerializedProperty _typeProperty;
        private SerializedProperty _weaponIDProperty;
        private SerializedProperty weaponType;
        
        private SerializedProperty _useConditionsProperty;
        
        private SerializedProperty _isAllowLevelExceedDataProperty;
        private SerializedProperty _maxLevelProperty;
        private SerializedProperty _defaultLevelProperty;
        private SerializedProperty _equipmentDataProperty;

        private bool IsWeapon => _typeProperty.enumValueIndex == (int)EquipmentType.Weapon;
        
        protected override void OnEnable()
        {
            base.OnEnable();

            _typeProperty = serializedObject.FindProperty("equipType");
            _weaponIDProperty = serializedObject.FindProperty("weaponID");
            //_rarityTypeProperty = serializedObject.FindProperty("rarityType");
            
            _useConditionsProperty = serializedObject.FindProperty("useConditions");

            _isAllowLevelExceedDataProperty = serializedObject.FindProperty("isAllowLevelExceedData");
            _maxLevelProperty = serializedObject.FindProperty("maxLevel");
            _defaultLevelProperty = serializedObject.FindProperty("defaultLevel");
            _equipmentDataProperty = serializedObject.FindProperty("equipmentData");
            weaponType = serializedObject.FindProperty("weaponType");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            float prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 220f;
            
            // TODO :
            DrawSettings();
            DrawUseConditions();
            DrawEquipmentData();
            
            EditorGUIUtility.labelWidth = prevLabelWidth;
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSettings()
        {
            if (!DrawFoldoutTitle("Setting"))
                return;

            CustomEditorUtility.DrawEnumToolbar(_typeProperty);
            CustomEditorUtility.DrawEnumToolbar(weaponType);

            if (IsWeapon)
                _weaponIDProperty.DrawPropertyField();

            //_rarityTypeProperty.DrawPropertyField();
        }
        
        private void DrawUseConditions()
        {
            if (!DrawFoldoutTitle("Use Condition"))
                return;

            _useConditionsProperty.DrawPropertyField();
        }

        private void DrawEquipmentData()
        {
            if (_equipmentDataProperty.arraySize == 0)
            {
                _equipmentDataProperty.arraySize++;
                _equipmentDataProperty.GetArrayElementAtIndex(0).FindPropertyRelative("level").intValue = 1;
            }
            
            if (!DrawFoldoutTitle("Data"))
                return;

            _isAllowLevelExceedDataProperty.DrawPropertyField();

            if (_isAllowLevelExceedDataProperty.boolValue)
            {
                _maxLevelProperty.DrawPropertyField();
            }
            else
            {
                GUI.enabled = false;
                
                int lastIndex = _equipmentDataProperty.arraySize - 1;
                var lastEquipmentData = _equipmentDataProperty.GetArrayElementAtIndex(lastIndex);

                _maxLevelProperty.intValue = lastEquipmentData.FindPropertyRelative("level").intValue;
                _maxLevelProperty.DrawPropertyField();

                GUI.enabled = true;
            }

            _defaultLevelProperty.DrawPropertyField();

            for (int i = 0; i < _equipmentDataProperty.arraySize; i++)
            {
                var property = _equipmentDataProperty.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginVertical("HelpBox");
                {
                    if (DrawRemovableLevelFoldout(_equipmentDataProperty, property, i, i != 0))
                    {
                        EditorGUILayout.EndVertical();
                        break;
                    }

                    EditorGUI.indentLevel += 1;

                    if (property.isExpanded)
                    {
                        property.NextVisible(true);
                        DrawAutoSortLevelProperty(_equipmentDataProperty, property, i, i != 0);

                        // Prefab
                        for (int j = 0; j < 2; j++)
                        {
                            property.NextVisible(false);
                            EditorGUILayout.PropertyField(property);
                        }
                        
                        // LV UP
                        for (int j = 0; j < 2; j++)
                        {
                            property.NextVisible(false);
                            EditorGUILayout.PropertyField(property);
                        }
                        
                        // STATs
                        property.NextVisible(false);
                        EditorGUILayout.PropertyField(property);
                    }

                    EditorGUI.indentLevel -= 1;
                }
                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Add New Level"))
            {
                // LV change
                int lastArraySize = _equipmentDataProperty.arraySize++;
                var prevElementalProperty = _equipmentDataProperty.GetArrayElementAtIndex(lastArraySize - 1);
                var newElementProperty = _equipmentDataProperty.GetArrayElementAtIndex(lastArraySize);
                int newElementLevel = prevElementalProperty.FindPropertyRelative("level").intValue + 1;

                newElementProperty.FindPropertyRelative("level").intValue = newElementLevel;
                newElementProperty.isExpanded = true;

                CustomEditorUtility.DeepCopySerializeReferenceArray(
                    newElementProperty.FindPropertyRelative("levelUpConditions"));
                CustomEditorUtility.DeepCopySerializeReferenceArray(
                    newElementProperty.FindPropertyRelative("levelUpCosts"));
            }
        }
    }
}