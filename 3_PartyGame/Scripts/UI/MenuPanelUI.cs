using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanelUI : PanelUI
{
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button characterButton;

    private void Awake()
    {
        gameStartButton.onClick.AddListener(() => PhotonNetwork.JoinLobby());
        characterButton.onClick.AddListener(() => Managers.UI.OpenPanel(PanelType.Character));
    }

    private void Start()
    {
        FindObjectOfType<PlayerCostumeData>().Save();
    }
}

