using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePanelUI : MonoBehaviour
{
    [SerializeField] private InfoPanelUI playerCharacterInfo;
    [SerializeField] private InfoPanelUI monsterInfo;

    [SerializeField] private Button finishedPlayerTurnButton;

    [SerializeField] private ClearPanelUI clearPanelUI;

    private void Start()
    {
        finishedPlayerTurnButton.onClick.AddListener(FinishedPlayerTurn);
    }

    private void Update()
    {
        if (Managers.Game.SelectedCharacter != null) 
            playerCharacterInfo.UpdateInfo(Managers.Game.SelectedCharacter);
        if (Managers.Game.Monster != null) 
            monsterInfo.UpdateInfo(Managers.Game.Monster);
        
        if (Managers.Game.GameMode == Define.GameMode.PlayerTurn)
        {
            finishedPlayerTurnButton.GetComponentInChildren<TextMeshProUGUI>().text = "다음 턴";
            finishedPlayerTurnButton.interactable = true;
        }
        else
        {
            finishedPlayerTurnButton.GetComponentInChildren<TextMeshProUGUI>().text = "잠시만\n기다리십시오.";
            finishedPlayerTurnButton.interactable = false;
        }
        
        foreach (Monster monster in Managers.Game.Monsters)
        {
            if (monster.State != Define.State.Idle)
            {
                finishedPlayerTurnButton.interactable = false;
            }
        }
    }

    private void FinishedPlayerTurn()
    {
        if (Managers.Game.PlayerCharacters.Count == 0 || Managers.Game.Monsters.Count == 0)
        {
            clearPanelUI.gameObject.SetActive(true);
            return;
        }

        if (Managers.Game.GameMode == Define.GameMode.MonsterTurn)
            return;
        
        foreach (PlayerCharacter playerCharacter in Managers.Game.PlayerCharacters) 
            playerCharacter.State = Define.State.Idle;
        
        Managers.Game.GameMode = Define.GameMode.MonsterTurn;
        StartCoroutine(Managers.Game.CoMovePath(Managers.Game.Monsters, Managers.Game.PlayerCharacters));
    }
    
}
