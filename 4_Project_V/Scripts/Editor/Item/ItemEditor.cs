using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace MyCustomEditor
{
    [CustomEditor(typeof(SO_Item), true)]
    public class ItemEditor : BaseObjectEditor
    {
        private SerializedProperty rarity;
        private SerializedProperty effect;
        private SerializedProperty type;
        private SerializedProperty _goldProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            rarity = serializedObject.FindProperty("rarity");
            effect = serializedObject.FindProperty("effect");
            type = serializedObject.FindProperty("type");
            _goldProperty = serializedObject.FindProperty("gold");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            float prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 220f;

            if (DrawFoldoutTitle("Type"))
            {
                CustomEditorUtility.DrawEnumToolbar(type);
            }

            if (DrawFoldoutTitle("rarity"))
            {
                rarity.DrawPropertyField();
            }

            if (DrawFoldoutTitle("effect"))
            {
                effect.DrawPropertyField();
            }

            _goldProperty.DrawPropertyField();
            
            EditorGUIUtility.labelWidth = prevLabelWidth;

            serializedObject.ApplyModifiedProperties();
        }

    }

}