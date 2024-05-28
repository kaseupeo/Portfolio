using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MyCustomEditor
{
    public class GameSystemWindow : EditorWindow
    {
        private static int _toolbarIndex = 0;
        private static Dictionary<Type, Vector2> _scrollPositionsByTypeDic = new Dictionary<Type, Vector2>();
        private static Vector2 _drawingEditorScrollPosition;
        private static Dictionary<Type, BaseObject> _selectedObjectsByTypeDic = new Dictionary<Type, BaseObject>();

        private readonly Dictionary<Type, BaseObjectDB> _dbByTypeDic = new Dictionary<Type, BaseObjectDB>();
        
        private Type[] _dbTypes;
        private string[] _dbTypeNames;
        
        private Editor _cashedEditor;

        private Texture2D _selectedBoxTexture;
        private GUIStyle _selectedBoxStyle;
        
        [MenuItem("Tools/Game System")]
        private static void OpenWindow()
        {
            var window = GetWindow<GameSystemWindow>("Game System");
            
            window.minSize = new Vector2(800, 700);
            window.Show();
        }

        private void OnEnable()
        {
            InitStyle();
            // TODO : 메뉴 추가 위치
            InitDB(new[]
            {
                typeof(Category), typeof(Effect), typeof(Skill), typeof(SkillTree),
                typeof(Stat), typeof(Equipment), typeof(Task), typeof(Quest), typeof(SO_CountableItem),
            });
        }

        private void OnGUI()
        {
            // _toolbarIndex = GUILayout.Toolbar(_toolbarIndex, _dbTypeNames);
            _toolbarIndex = GUILayout.SelectionGrid(_toolbarIndex, _dbTypeNames, 4);
            EditorGUILayout.Space(4f);
            CustomEditorUtility.DrawUnderline();
            EditorGUILayout.Space(4f);

            DrawDB(_dbTypes[_toolbarIndex]);
        }

        private void OnDisable()
        {
            DestroyImmediate(_cashedEditor);
            DestroyImmediate(_selectedBoxTexture);
        }

        private void InitStyle()
        {
            _selectedBoxTexture = new Texture2D(1, 1);
            _selectedBoxTexture.SetPixel(0, 0, new Color(0.31f, 0.40f, 0.50f));
            _selectedBoxTexture.Apply();
            _selectedBoxTexture.hideFlags = HideFlags.DontSave;

            _selectedBoxStyle = new GUIStyle();
            _selectedBoxStyle.normal.background = _selectedBoxTexture;
        }

        private void InitDB(Type[] dataTypes)
        {
            if (_dbByTypeDic.Count == 0)
            {
                if (!AssetDatabase.IsValidFolder("Assets/Resources/SO/DB")) 
                    AssetDatabase.CreateFolder("Assets/Resources/SO", "DB");

                foreach (Type type in dataTypes)
                {
                    var db = AssetDatabase.LoadAssetAtPath<BaseObjectDB>($"Assets/Resources/SO/DB/{type.Name}DB.asset");

                    if (db == null)
                    {
                        db = CreateInstance<BaseObjectDB>();
                        AssetDatabase.CreateAsset(db, $"Assets/Resources/SO/DB/{type.Name}DB.asset");
                        AssetDatabase.CreateFolder("Assets/Resources/SO", type.Name);
                    }

                    _dbByTypeDic[type] = db;
                    _scrollPositionsByTypeDic[type] = Vector2.zero;
                    _selectedObjectsByTypeDic[type] = null;
                }

                _dbTypeNames = dataTypes.Select(x => x.Name).ToArray();
                _dbTypes = dataTypes;
            }
        }

        private void DrawDB(Type dataType)
        {
            var db = _dbByTypeDic[dataType];
            
            AssetPreview.SetPreviewTextureCacheSize(Mathf.Max(32, 32 + db.Count));

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(300f));
                {
                    GUI.color = Color.green;

                    if (GUILayout.Button($"New {dataType.Name}"))
                    {
                        var guid = Guid.NewGuid();
                        var newData = CreateInstance(dataType) as BaseObject;

                        if (dataType.BaseType.Name == "SO_Item")
                            dataType.BaseType.BaseType.GetField("codeName", BindingFlags.NonPublic | BindingFlags.Instance)
                                .SetValue(newData, guid.ToString());
                        else
                            dataType.BaseType.GetField("codeName", BindingFlags.NonPublic | BindingFlags.Instance)
                            .SetValue(newData, guid.ToString());

                        AssetDatabase.CreateAsset(newData,
                            $"Assets/Resources/SO/{dataType.Name}/{dataType.Name.ToUpper()}_{guid}.asset");

                        db.Add(newData);

                        EditorUtility.SetDirty(db);
                        AssetDatabase.SaveAssets();

                        _selectedObjectsByTypeDic[dataType] = newData;
                    }
                    
                    GUI.color = Color.red;
                    
                    if (GUILayout.Button($"Remove Last {dataType.Name}"))
                    {
                        var lastData = db.Count > 0 ? db.DataList.Last() : null;

                        if (lastData)
                        {
                            db.Remove(lastData);

                            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(lastData));
                            EditorUtility.SetDirty(db);
                            AssetDatabase.SaveAssets();
                        }
                    }
                    
                    GUI.color = Color.cyan;

                    if (GUILayout.Button("Sort By Name"))
                    {
                        db.SortByCodeName();

                        EditorUtility.SetDirty(db);
                        AssetDatabase.SaveAssets();
                    }
                    
                    GUI.color = Color.white;

                    EditorGUILayout.Space(2f);
                    CustomEditorUtility.DrawUnderline();
                    EditorGUILayout.Space(4f);

                    _scrollPositionsByTypeDic[dataType] = EditorGUILayout.BeginScrollView(
                        _scrollPositionsByTypeDic[dataType], false, true, GUIStyle.none, GUI.skin.verticalScrollbar,
                        GUIStyle.none);
                    {
                        foreach (BaseObject data in db.DataList)
                        {
                            float labelWidth = data.Icon != null ? 200f : 245f;
                            var style = _selectedObjectsByTypeDic[dataType] == data ? _selectedBoxStyle : GUIStyle.none;

                            EditorGUILayout.BeginHorizontal(style, GUILayout.Height(40f));
                            {
                                if (data.Icon)
                                {
                                    var preview = AssetPreview.GetAssetPreview(data.Icon);
                                    GUILayout.Label(preview, GUILayout.Height(40f), GUILayout.Width(40f));
                                }

                                EditorGUILayout.LabelField(data.CodeName, GUILayout.Width(labelWidth),
                                    GUILayout.Height(40f));

                                EditorGUILayout.BeginVertical();
                                {
                                    EditorGUILayout.Space(10f);

                                    GUI.color = Color.red;

                                    if (GUILayout.Button("\u00d7", GUILayout.Width(20f)))
                                    {
                                        db.Remove(data);

                                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(data));
                                        EditorUtility.SetDirty(db);
                                        AssetDatabase.SaveAssets();
                                    }
                                }
                                EditorGUILayout.EndVertical();
                                
                                GUI.color = Color.white;
                            }
                            EditorGUILayout.EndHorizontal();

                            if (data == null)
                                break;

                            var lastRect = GUILayoutUtility.GetLastRect();

                            if (Event.current.type == EventType.MouseDown && lastRect.Contains(Event.current.mousePosition))
                            {
                                _selectedObjectsByTypeDic[dataType] = data;
                                _drawingEditorScrollPosition = Vector2.zero;
                                
                                Event.current.Use();
                            }
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();

                if (_selectedObjectsByTypeDic[dataType])
                {
                    _drawingEditorScrollPosition = EditorGUILayout.BeginScrollView(_drawingEditorScrollPosition);
                    {
                        EditorGUILayout.Space(2f);
                        Editor.CreateCachedEditor(_selectedObjectsByTypeDic[dataType], null, ref _cashedEditor);
                        _cashedEditor.OnInspectorGUI();
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}