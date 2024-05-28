using UnityEditor;
using UnityEngine;

namespace MyCustomEditor
{
    [CustomEditor(typeof(Quest))]
    public class QuestEditor : BaseObjectEditor
    {
        private SerializedProperty _acceptionConditionsProperty;
        private SerializedProperty _cancelConditionsProperty;

        private SerializedProperty _taskGroupsProperty;
        private SerializedProperty _rewardsProperty;

        private SerializedProperty _useAutoCompleteProperty;
        private SerializedProperty _isCancelableProperty;
        private SerializedProperty _isSavableProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            _acceptionConditionsProperty = serializedObject.FindProperty("acceptionConditions");
            _cancelConditionsProperty = serializedObject.FindProperty("cancelConditions");

            _taskGroupsProperty = serializedObject.FindProperty("taskGroups");
            _rewardsProperty = serializedObject.FindProperty("rewards");

            _useAutoCompleteProperty = serializedObject.FindProperty("useAutoComplete");
            _isCancelableProperty = serializedObject.FindProperty("isCancelable");
            _isSavableProperty = serializedObject.FindProperty("isSavable");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();

            float prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 220f;

            DrawConditions();
            DrawTask();
            DrawReward();
            DrawOption();
            
            EditorGUIUtility.labelWidth = prevLabelWidth;

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawConditions()
        {
            if (!DrawFoldoutTitle("Conditions"))
                return;

            _acceptionConditionsProperty.DrawPropertyField();
            _cancelConditionsProperty.DrawPropertyField();
        }

        private void DrawTask()
        {
            if (!DrawFoldoutTitle("Task"))
                return;

            _taskGroupsProperty.DrawPropertyField();
        }

        private void DrawReward()
        {
            if (!DrawFoldoutTitle("Reward"))
                return;

            _rewardsProperty.DrawPropertyField();
        }

        private void DrawOption()
        {
            if (!DrawFoldoutTitle("Option"))
                return;

            _useAutoCompleteProperty.DrawPropertyField();
            _isCancelableProperty.DrawPropertyField();
            _isSavableProperty.DrawPropertyField();
        }
    }
}