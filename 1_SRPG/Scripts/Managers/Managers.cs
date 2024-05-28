using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    #region 싱글톤

    private static Managers _instance;

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
        if (_instance == null)
        {
            GameObject go = GameObject.Find("Managers");

            if (go == null)
            {
                go = new GameObject("Managers");
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            _instance = go.GetComponent<Managers>();
        }
    }

    #endregion

    private SceneLoadManager _scene = new SceneLoadManager();
    private GameManager _game = new GameManager();
    private PoolManager _pool = new PoolManager();
    private MapManager _map = new MapManager();

    public static SceneLoadManager Scene => Instance._scene;
    public static GameManager Game => Instance._game;
    public static PoolManager Pool => Instance._pool;
    public static MapManager Map => Instance._map;

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, Game.IsFullScreenMode);
        Init();
    }
    
    public static void Clear()
    {
        Scene.Clear();
        Game.Clear();
        Pool.Clear();
        Map.Clear();
    }
}
