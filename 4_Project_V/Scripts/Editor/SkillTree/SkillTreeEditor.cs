using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNodeEditor;
using XNode;

namespace MyCustomEditor
{
    [CustomEditor(typeof(SkillTree))]
    public class SkillTreeEditor : BaseObjectEditor
    {
        private SerializedProperty _graphProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            _graphProperty = serializedObject.FindProperty("graph");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            // graph가 없으면 자동으로 만들어줌
            if (_graphProperty.objectReferenceValue == null)
            {
                var targetObject = serializedObject.targetObject;
                var newGraph = CreateInstance<SkillTreeGraph>();
                newGraph.name = "Skill Tree Graph";

                // NodeGraph도 ScriptableObject Type이므로 일반적인 자료형처럼 Serialize를 할 수 없기에
                // BaseObject의 하위 Asset으로 만들어서 불러오는 방식으로 Serialize함
                AssetDatabase.AddObjectToAsset(newGraph, targetObject);
                AssetDatabase.SaveAssets();

                _graphProperty.objectReferenceValue = newGraph;
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Open Window", GUILayout.Height(50f)))
                NodeEditorWindow.Open(_graphProperty.objectReferenceValue as NodeGraph);

            serializedObject.ApplyModifiedProperties();
        }
    }

}