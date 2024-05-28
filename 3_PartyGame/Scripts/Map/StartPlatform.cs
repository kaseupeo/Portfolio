using System;
using UnityEngine;

public class StartPlatform : MonoBehaviour
{
    private void OnEnable()
    {
        Managers.Game.CountDownAction += OnGameCountDownAction;
    }

    private void OnDestroy()
    {
        Managers.Game.CountDownAction -= OnGameCountDownAction;
    }

    private void OnGameCountDownAction(int time)
    {
        if (time <= 0) 
            Destroy(gameObject);
    }
}