using UnityEngine;
using UnityEngine.EventSystems;

public class Managers : MonoBehaviour
{
    #region 싱글톤

    private static Managers _instance;
    private static bool _isQuitting;
    
    public static Managers Instance
    {
        get
        {
            Init();
            return _instance;
        }
    }

    private static void Init()
    {
        if (!_isQuitting && _instance == null)
        {
            GameObject go = GameObject.Find("Managers");
            
            if (go == null) 
                go = new GameObject("Managers");

            DontDestroyOnLoad(go);
            _instance = go.GetOrAddComponent<Managers>();
        }
    }

    #endregion

    private GameManager _game = new();
    private PoolManager _pool = new();
    private ResourceManager _resource = new();
    private UIManager _ui = new();
    private SceneLoadManager _scene = new();
    private InputManager _input = new();
    private SoundManager _sound = new();
    private DataManager _data = new(); 
    private QuestManager _quest = new();
    private BattleManager _battle = new(); 

    public static GameManager Game => Instance?._game;
    public static PoolManager Pool => Instance?._pool;
    public static ResourceManager Resource => Instance?._resource;
    public static UIManager UI => Instance?._ui;
    public static SceneLoadManager Scene => Instance?._scene;
    public static InputManager Input => Instance?._input;
    public static SoundManager Sound => Instance?._sound;
    public static DataManager Data => Instance?._data;
    public static QuestManager Quest => Instance?._quest;
    public static BattleManager Battle => Instance?._battle;
    
    private void Awake()
    {
        Init();
        Input.Init();
        Data.Init();
        UI.Init();
        Battle.Init(); 
    }

    public static void Clear()
    {
        Input.Clear();
    }
    
    private void OnApplicationQuit()
    {
        Quest.Save();
        Clear();
        _isQuitting = true;
    }

    public void Quit()
    {
        _isQuitting = true;
    }
}