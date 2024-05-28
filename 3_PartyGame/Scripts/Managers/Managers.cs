using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
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
            {
                go = new GameObject("Managers");
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            _instance = go.GetComponent<Managers>();
        }
    }

    private GameManager _game = new();
    private UIManager _ui = new();

    public static GameManager Game = Instance?._game;
    public static UIManager UI = Instance?._ui;

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }
}

