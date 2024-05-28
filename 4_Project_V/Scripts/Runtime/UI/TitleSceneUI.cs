using System;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneUI : BaseUI
{
    private void Start()
    {
        buttons["GameStart"].onClick.AddListener(() => Managers.Scene.LoadScene(Define.SceneType.GameScene));
        buttons["ExitButton"].onClick.AddListener(()=> Managers.Game.Quit());
    }
}