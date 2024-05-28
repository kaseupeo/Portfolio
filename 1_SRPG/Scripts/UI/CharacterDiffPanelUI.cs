using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDiffPanelUI : MonoBehaviour
{
    [SerializeField] private ScrollbarUI characterCountUI;
    [SerializeField] private ScrollbarUI monsterRateUI;
    
    private void Update()
    {
        SetDiff();
    }

    private void SetDiff()
    {
        Managers.Game.MaxPlayerCharacter = characterCountUI.Index + 1;
        Managers.Game.MonsterRate = monsterRateUI.Index + 1;
    }


}
