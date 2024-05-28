using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TitlePanelUI : PanelUI
{
    [SerializeField] private Button startButton;

    private void Awake()
    {

        startButton.onClick.AddListener(() =>
        {
            PhotonNetwork.LocalPlayer.NickName = $"Player_{Random.Range(1, 1000)}";
            PhotonNetwork.ConnectUsingSettings();
            InteractiveButton(false);
        });
    }
    
    private void OnEnable()
    {
        InteractiveButton(true);
    }

    private void InteractiveButton(bool isInteractive)
    {
        startButton.interactable = isInteractive;
    }
}

