using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LobbyPanelUI : PanelUI
{
    [SerializeField] private GameObject roomButtonPrefab;
    [SerializeField] private RectTransform content;
    
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button backButton;

    [SerializeField] private RoomSettingPopupUI roomSettingPopup;

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        roomSettingPopup.gameObject.SetActive(false);
        
        createRoomButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        createRoomButton.onClick.AddListener(CreateRoom);
        backButton.onClick.AddListener(LeaveLobby);
    }
    
    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (Transform child in content.transform)
        {
            if (child == content.transform)
                continue;
            
            Destroy(child.gameObject);
        }
        
        for (var i = 0; i < roomList.Count; i++)
        {
            var room = roomList[i];
            Button go = Instantiate(roomButtonPrefab, content, false).GetComponent<Button>();
            go.name = room.Name;
            go.onClick.AddListener(() => PhotonNetwork.JoinRoom(room.Name));

            var texts = go.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = i.ToString();
            texts[1].text = room.Name;
            texts[2].text = $"{room.PlayerCount} / {room.MaxPlayers}";
        }
    }

    private void CreateRoom()
    {
        roomSettingPopup.gameObject.SetActive(true);
        
        createRoomButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        
        createRoomButton.onClick.AddListener(()
            => PhotonNetwork.CreateRoom(roomSettingPopup.RoomName,
                new RoomOptions { MaxPlayers = roomSettingPopup.MaxPlayer }));
        backButton.onClick.AddListener(Init);
    }

    private void LeaveLobby()
    {
        PhotonNetwork.LeaveLobby();
    }
}