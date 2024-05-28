using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public abstract class BaseScene : MonoBehaviour
{
    protected Define.SceneType _sceneType;

    public Define.SceneType SceneType => _sceneType;

    private void Awake()
    {
        Init();

    }

    protected virtual void Init()
    {

    }

    public abstract void Clear();
}