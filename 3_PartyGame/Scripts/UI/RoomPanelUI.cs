using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RoomPanelUI : PanelUI
{
    [SerializeField] private RectTransform slotParent;
    [SerializeField] private GameObject characterSlotPrefab;
    [SerializeField] private TextMeshProUGUI roomName;
    
    [SerializeField] private Button readyButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] private Button startButton;
    
    private Dictionary<int, PlayerData> _playerDic = new();
    private Dictionary<int, bool> _playerReadyDic = new();
    
    private void Awake()
    {
        readyButton.onClick.AddListener(ReadyGame);
        leaveButton.onClick.AddListener(LeaveGame);
        startButton.onClick.AddListener(StartGame);
    }

    private void OnEnable()
    {
        if (!PhotonNetwork.InRoom)
            return;
        
        foreach (Transform child in slotParent) 
            Destroy(child.gameObject);

        if (PhotonNetwork.IsMasterClient)
            _playerReadyDic = new();
        
        roomName.text = PhotonNetwork.CurrentRoom.Name;

        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            JoinPlayer(player);

            if (player.CustomProperties.TryGetValue("Ready", out var customProperty))
                SetPlayerReady(player.ActorNumber, (bool)customProperty);
        }

        startButton.gameObject.SetActive(false);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void OnDisable()
    {
        _playerDic.Clear();
    }

    public void JoinPlayer(Player newPlayer)
    {
        PlayerData playerData = Instantiate(characterSlotPrefab, slotParent, false).GetComponent<PlayerData>();
        playerData.PlayerName.text = newPlayer.NickName;
        
        _playerDic.Add(newPlayer.ActorNumber, playerData.transform.GetComponent<PlayerData>());
        // playerData.UpdateImage();
        
        if (PhotonNetwork.IsMasterClient) 
            _playerReadyDic.Add(newPlayer.ActorNumber, false);
        
        SortPlayers();
    }

    public void LeavePlayer(Player gonePlayer)
    {
        if (_playerDic.ContainsKey(gonePlayer.ActorNumber))
        {
            Destroy(_playerDic[gonePlayer.ActorNumber].gameObject);
            _playerDic.Remove(gonePlayer.ActorNumber);
        }

        if (PhotonNetwork.IsMasterClient) 
            _playerReadyDic.Remove(gonePlayer.ActorNumber);
        
        SortPlayers();
    }
    
    public void SetPlayerReady(int actorNumber, bool isReady)
    {
        _playerDic[actorNumber].ReadyImage.gameObject.SetActive(isReady);

        if (PhotonNetwork.IsMasterClient)
        {
            _playerReadyDic[actorNumber] = isReady;
            readyButton.gameObject.SetActive(!_playerReadyDic.Values.All(x => x));
            startButton.gameObject.SetActive(_playerReadyDic.Values.All(x => x));
        }
    }

    public void SetPlayerCostumeDate(Player player, Hashtable changedProps)
    {
        Dictionary<CostumeType, int> dic = new();
            
        for (int i = 0; i < Enum.GetValues(typeof(CostumeType)).Length; i++)
        {
            if (changedProps.TryGetValue(((CostumeType)i).ToString(), out var changedProp)) 
                dic[(CostumeType)i] = (int)changedProp;
        }

        Managers.Game.PlayersCostumeDic[player.ActorNumber] = dic;
    }

    // public void SetPlayerImage(Player player, byte[] textureBytes)
    // {
    //     Texture2D receivedTexture = new Texture2D(1, 1);
    //     receivedTexture.LoadImage(Utils.Decompress(textureBytes));
    //     _playerDic[player.ActorNumber].PlayerImage.texture = receivedTexture;
    // }
    
    private void SortPlayers()
    {
        foreach (var pair in _playerDic) 
            pair.Value.transform.SetSiblingIndex(pair.Key);
    }
    
    private void ReadyGame()
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;
        Hashtable customProperty = PhotonNetwork.LocalPlayer.CustomProperties;

        customProperty["Ready"] = customProperty["Ready"] != null && !(bool)customProperty["Ready"];
        
        localPlayer.SetCustomProperties(customProperty);
        leaveButton.interactable = !(bool)customProperty["Ready"];
    }
    
    private void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void StartGame()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}