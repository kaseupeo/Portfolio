using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearPanelUI : MonoBehaviour
{
    [SerializeField] private Button replayButton;
    [SerializeField] private Button lobbyButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private Sprite clearSprite;
    [SerializeField] private Sprite overSprite;
    [SerializeField] private Image childImage;
    
    
    private Image _image;
    
    private void Start()
    {
        replayButton.onClick.AddListener(Replay);
        lobbyButton.onClick.AddListener(Lobby);
        quitButton.onClick.AddListener(() => Managers.Game.GameQuit());
    }

    public void OnEnable()
    {
        _image = GetComponent<Image>();
        
        if (Managers.Game.Monsters.Count == 0)
        {
            _image.color = new Color(1, 1, 1, 1);
            childImage.sprite = clearSprite;
        }

        if (Managers.Game.PlayerCharacters.Count == 0)
        {
            _image.color = new Color(1, 0, 0, 1);
            childImage.sprite = overSprite;
        }

        StartCoroutine(OnButton());
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
    
    private IEnumerator OnButton()
    {
        yield return new WaitForSeconds(2f);
        replayButton.gameObject.SetActive(true);
        lobbyButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }
}
