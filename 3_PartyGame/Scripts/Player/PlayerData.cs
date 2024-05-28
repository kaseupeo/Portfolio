using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private RawImage playerImage;
    [SerializeField] private Image readyImage;

    public TextMeshProUGUI PlayerName => playerName;
    public RawImage PlayerImage { get => playerImage; set => playerImage = value; }
    public Image ReadyImage => readyImage;

    private void Start()
    {
        Texture2D textureToSend = RenderTextureToTexture2D((RenderTexture)playerImage.mainTexture);

        // photonView.RPC(nameof(ReceiveTextureRPC), RpcTarget.Others, textureToSend.EncodeToPNG());
    }

    public void UpdateImage()
    {
        RenderTexture renderTexture = (RenderTexture)playerImage.mainTexture;
        Texture2D textureToSend = new Texture2D(renderTexture.width, renderTexture.height);
        
        RenderTexture.active = renderTexture;
        textureToSend.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        textureToSend.Apply();
        RenderTexture.active = null;

        byte[] textureBytes = textureToSend.EncodeToPNG();
        Hashtable playerProperties = new Hashtable();

        playerProperties["PlayerImage"] = Utils.Compress(textureBytes);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }

    private Texture2D RenderTextureToTexture2D(RenderTexture renderTexture)
    {
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        return texture;
    }

    [PunRPC]
    public void ReceiveTextureRPC(byte[] textureData, PhotonMessageInfo info)
    {
        if (info.Sender.ActorNumber != photonView.Owner.ActorNumber)
            return;
        
        Texture2D receivedTexture = new Texture2D(1, 1);
        receivedTexture.LoadImage(textureData);
    }
}