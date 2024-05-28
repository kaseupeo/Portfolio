using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button gameSettingsButton;
    [SerializeField] private Button gameQuitButton;

    [SerializeField] private GameObject settingsMenu;
    
    private void Start()
    {
        gameStartButton.onClick.AddListener(() => Managers.Scene.LoadScene(Define.SceneType.GameScene));
        gameSettingsButton.onClick.AddListener(() => settingsMenu.SetActive(true));
        gameQuitButton.onClick.AddListener(() => Managers.Game.GameQuit());
    }
}
