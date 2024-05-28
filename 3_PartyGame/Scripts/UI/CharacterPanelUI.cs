using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterPanelUI : PanelUI
{
    [SerializeField] private PlayerCostumeData playerCostumeData;
    [SerializeField] private RectTransform prefabParent;
    [SerializeField] private ButtonUI buttonPrefab;
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button backButton;
    
    private void Awake()
    {
        saveButton.onClick.AddListener(Save);
        backButton.onClick.AddListener(() => Managers.UI.OpenPanel(PanelType.Menu));
    }

    private void Start()
    {
        foreach (var pair in Managers.Game.CostumeDic)
        {
            var button = Instantiate(buttonPrefab, prefabParent, false);
            button.Costume = playerCostumeData.FindByType(pair.Key);
            button.Init();
        }
    }

    private void OnEnable()
    {
        playerName.text = "";
        playerName.placeholder.GetComponent<TextMeshProUGUI>().text = PhotonNetwork.LocalPlayer.NickName;
    }

    private void Save()
    {
        if (!string.IsNullOrEmpty(playerName.text)) 
            PhotonNetwork.LocalPlayer.NickName = playerName.text;

        playerCostumeData.Save();
    }
}