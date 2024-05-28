using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class LobbyScene : MonoBehaviourPunCallbacks
{
    [SerializeField] private TitlePanelUI title;
    [SerializeField] private MenuPanelUI menu;
    [SerializeField] private CharacterPanelUI character;
    [SerializeField] private LobbyPanelUI lobby;
    [SerializeField] private RoomPanelUI room;

    private ClientState _state = 0;
    
    private void Awake()
    {
        Managers.UI.PanelDic.Add(PanelType.Title, title);
        Managers.UI.PanelDic.Add(PanelType.Menu, menu);
        Managers.UI.PanelDic.Add(PanelType.Character, character);
        Managers.UI.PanelDic.Add(PanelType.Lobby, lobby);
        Managers.UI.PanelDic.Add(PanelType.Room, room);
        
        Managers.UI.OpenPanel(PanelType.Title);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        Managers.UI.PanelDic.Clear();
    }

    private void Update()
    {
        if (_state != PhotonNetwork.NetworkClientState)
        {
            _state = PhotonNetwork.NetworkClientState;
            Debug.Log($"<color=yellow>상태</color> - {_state}");
        }
    }
    
    public override void OnConnected()
    {
        Managers.UI.OpenPanel(PanelType.Menu);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"<color=red>연결 끊김</color> - {cause}");
        // OpenPanel(PanelType.Title);
    }

    public override void OnJoinedLobby() 
        =>  Managers.UI.OpenPanel(PanelType.Lobby);
    public override void OnLeftLobby() 
        =>  Managers.UI.OpenPanel(PanelType.Menu);

    public override void OnRoomListUpdate(List<RoomInfo> roomList) 
        => lobby.UpdateRoomList(roomList);

    public override void OnCreatedRoom() =>  Managers.UI.OpenPanel(PanelType.Room);
    public override void OnJoinedRoom()
    {
        Managers.UI.OpenPanel(PanelType.Room);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"<color=red>방입장 실패</color> - {message}");
        Managers.UI.OpenPanel(PanelType.Menu);
    }
    public override void OnLeftRoom() 
        =>  Managers.UI.OpenPanel(PanelType.Menu);
    public override void OnPlayerEnteredRoom(Player newPlayer) 
        => room.JoinPlayer(newPlayer);
    public override void OnPlayerLeftRoom(Player otherPlayer) 
        => room.LeavePlayer(otherPlayer);
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        room.SetPlayerReady(targetPlayer.ActorNumber, (bool)changedProps["Ready"]);
        room.SetPlayerCostumeDate(targetPlayer, changedProps);
        // room.SetPlayerImage(targetPlayer, (byte[])changedProps["PlayerImage"]);
    }
}