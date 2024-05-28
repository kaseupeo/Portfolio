using System;
using UnityEngine;

[Serializable]
public struct EquipmentData
{
    public int level;

    [UnderlineTitle("프리팹")] 
    public GameObject prefab;
    [Tooltip("장비 위치")]
    public string position;
    
    [UnderlineTitle("Level Up")] 
    [SerializeReference, SubclassSelector, Tooltip("장비 레벨업을 위한 조건")]
    public CreatureCondition[] levelUpConditions;
    [SerializeReference, SubclassSelector, Tooltip("장비 레벨업을 위한 비용")]
    public Cost[] levelUpCosts;

    [UnderlineTitle("스탯")] 
    public StatSelector[] stats;
}