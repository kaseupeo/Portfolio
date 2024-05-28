using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

public enum BossType
{
    Demonking,
    WormMonster,
}
public class BattleManager
{
    public UnityEvent onBossMeet;
    public BossMonsterHUD bossHUD;
    [SerializeField] bool isEnter;
    private Dictionary<BossType, Creature> bossDic = new();
    
    public IReadOnlyDictionary<BossType, Creature> BossDic => bossDic;
    public bool IsEnter { get { return isEnter; } set { isEnter = value; } }


    public void Init()
    {
       
    }

    public void PlayerEnterBossRoom(Creature boss)
    {
        Debug.Log($"{bossHUD.name}");
        bossHUD.SetData(boss);
        OpenBossHUDPanel();
    }
    public void OpenBossHUDPanel()
    {
        isEnter = !isEnter;
        bossHUD.gameObject.SetActive(isEnter);
       
    }
  

}
