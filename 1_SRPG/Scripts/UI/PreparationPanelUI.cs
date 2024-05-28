using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreparationPanelUI : MonoBehaviour
{
    [SerializeField] private InfoPanelUI playerCharacterInfo;

    [SerializeField] private Button battleStartButton;

    private void Start()
    {
        battleStartButton.onClick.AddListener(BattleStart);
    }

    private void Update()
    {
        if (Managers.Game.SelectedCharacter != null)
        {
            playerCharacterInfo.UpdateInfo(Managers.Game.SelectedCharacter);
        }
        battleStartButton.interactable = Managers.Game.PlayerCharacters.Count != 0;
    }

    private void BattleStart()
    {
        Managers.Game.SelectedCharacter = null;
        Managers.Game.GameMode = Define.GameMode.PlayerTurn;
        Managers.Game.GenerateMonster();
        HideMapTile();
    }

    private void HideMapTile()
    {
        GameObject startGround = GameObject.FindWithTag("StartGround");
        if (startGround == null)
            return;

        foreach (SpriteRenderer spriteRenderer in startGround.GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.color = new Color(1, 1, 1, 0);
        }

        GameObject monsterGround = GameObject.FindWithTag("MonsterGround");
        if (monsterGround == null)
            return;

        foreach (SpriteRenderer spriteRenderer in monsterGround.GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.color = new Color(1, 1, 1, 0);
        }
    }
}
