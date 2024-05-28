using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanelUI : MonoBehaviour
{
    [SerializeField] private Button playGameButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button lobbyButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private GameObject settingsMenu;
    
    private void Start()
    {
        playGameButton.onClick.AddListener(PlayGame);
        replayButton.onClick.AddListener(Replay);
        settingsButton.onClick.AddListener(() => settingsMenu.SetActive(!settingsMenu.activeSelf));
        lobbyButton.onClick.AddListener(Lobby);
        quitButton.onClick.AddListener(() => Managers.Game.GameQuit());
    }

    public void PlayGame()
    {
        settingsMenu.SetActive(false);
        gameObject.SetActive(false);
    }

    private void Replay()
    {
        Managers.Game.Clear();
        Managers.Map.Clear();
        Managers.Game.Init();
        Managers.Map.Init();
        Managers.Scene.LoadScene(Define.SceneType.GameScene);
    }

    private void Lobby()
    {
        Managers.Game.Clear();
        Managers.Map.Clear();
        Managers.Scene.LoadScene(Define.SceneType.LobbyScene);
    }
}
