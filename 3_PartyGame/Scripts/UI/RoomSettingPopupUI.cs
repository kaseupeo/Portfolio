using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomSettingPopupUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField roomName;
    [SerializeField] private TMP_InputField maxPlayer;

    public string RoomName => roomName.text;
    public int MaxPlayer
    {
        get
        {
            if (int.TryParse(maxPlayer.text, out int result))
                return result is >= 0 and <= Define.MaxPlayer ? int.Parse(maxPlayer.text) : Define.MaxPlayer;
            
            return Define.MaxPlayer;
        }
    }
}