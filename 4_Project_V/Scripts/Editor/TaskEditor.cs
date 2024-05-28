using UnityEditor;

namespace MyCustomEditor
{
    [CustomEditor(typeof(Task))]
    public class TaskEditor : BaseObjectEditor
    {
        private SerializedProperty _actionProperty;
        
        private SerializedProperty _targetsProperty;

        private SerializedProperty _initialSuccessValueProperty;
        private SerializedProperty _needSuccessToCompleteProperty;
        private SerializedProperty _canReceiveReportsDuringCompleteProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            _actionProperty = serializedObject.FindProperty("action");

            _targetsProperty = serializedObject.FindProperty("targets");

            _initialSuccessValueProperty = serializedObject.FindProperty("initialSuccessValue");
            _needSuccessToCompleteProperty = serializedObject.FindProperty("needSuccessToComplete");
            _canReceiveReportsDuringCompleteProperty = serializedObject.FindProperty("canReceiveReportsDuringComplete");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();

            float prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 220f;

            DrawAction();
            DrawTarget();
            DrawSettings();
            
            EditorGUIUtility.labelWidth = prevLabelWidth;

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAction()
        {
            if (!DrawFoldoutTitle("Action"))
                return;

            _actionProperty.DrawPropertyField();
        }

        private void DrawTarget()
        {
            if (!DrawFoldoutTitle("Target"))
                return;

            _targetsProperty.DrawPropertyField();
        }

        private void DrawSettings()
        {
            if (!DrawFoldoutTitle("Setting"))
                return;

            _initialSuccessValueProperty.DrawPropertyField();
            _needSuccessToCompleteProperty.DrawPropertyField();
            _canReceiveReportsDuringCompleteProperty.DrawPropertyField();
        }
    }
}