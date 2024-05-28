using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager
{
    public Action<int> CountDownAction;
    public Action<Player> AlertAction;
    public Action FinishAction;

    public GameObject Player { get; set; }
    public Stack<Player> PlayerRankStack { get; set; } = new();
    public Dictionary<int, Dictionary<CostumeType, int>> PlayersCostumeDic { get; set; } = new();
    public Dictionary<CostumeType, int> CostumeDic { get; set; } = new();
    public List<GameObject> CamList { get; set; } = new();

    public void Init()
    {
        PlayerRankStack = new();
    }
    
    public void FinishGame()
    {
        PhotonNetwork.LoadLevel("LobbyScene");
        
        if (PhotonNetwork.InRoom) 
            PhotonNetwork.LeaveRoom();
        if (PhotonNetwork.InLobby) 
            PhotonNetwork.LeaveLobby();
    }
}