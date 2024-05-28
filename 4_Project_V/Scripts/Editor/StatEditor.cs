using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyCustomEditor
{
    [CustomEditor(typeof(Stat))]
    public class StatEditor : BaseObjectEditor
    {
        private SerializedProperty _isPercentTypeProperty;
        private SerializedProperty _maxValueProperty;
        private SerializedProperty _minValueProperty;
        private SerializedProperty _defaultValueProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _isPercentTypeProperty = serializedObject.FindProperty("isPercentType");
            _maxValueProperty = serializedObject.FindProperty("maxValue");
            _minValueProperty = serializedObject.FindProperty("minValue");
            _defaultValueProperty = serializedObject.FindProperty("defaultValue");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            if (DrawFoldoutTitle("스탯 설정"))
            {
                _isPercentTypeProperty.DrawPropertyField();
                _maxValueProperty.DrawPropertyField();
                _minValueProperty.DrawPropertyField();
                _defaultValueProperty.DrawPropertyField();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}