using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ModePanelUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> diffUI;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    [SerializeField] private GameObject panel;
    
    private int _currentPage;
    
    private void Start()
    {
        _currentPage = 0;
        prevButton.onClick.AddListener(PrevPage);
        nextButton.onClick.AddListener(NextPage);
    }

    private void OnEnable()
    {
        diffUI[_currentPage].gameObject.SetActive(true);
    }

    private void PrevPage()
    {
        diffUI[_currentPage].gameObject.SetActive(false);
        
        if (_currentPage <= 0)
        {
            Managers.Scene.LoadScene(Define.SceneType.LobbyScene);
            return;
        }

        _currentPage--;
        diffUI[_currentPage].SetActive(true);
    }

    private void NextPage()
    {
        diffUI[_currentPage].gameObject.SetActive(false);

        if (_currentPage >= diffUI.Count - 1)
        {
            gameObject.SetActive(false);
            panel.SetActive(true);
            Managers.Map.GenerateOverlayTile(Managers.Game.Map);
            Managers.Game.ShowStartTile();
            Camera.main.transform.position = new Vector3(0, 0, -10);
            return;
        }

        _currentPage++;
        diffUI[_currentPage].SetActive(true);
    }
}
