using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemType = Define.ItemType;

[CreateAssetMenu(fileName = "New BeginDropItem", menuName = "Non-EquipItems")]
public  class SO_Item : BaseObject
{
    [SerializeField] private ItemEffectData[] effect;
    [SerializeField] private SO_Rarity rarity;
    [SerializeField] private ItemType type;
    [SerializeField] private float gold;

    public ItemType Type => type;
    public ItemEffectData[] Effect => effect;
    public SO_Rarity Rarity => rarity;
    public float Gold => gold;
}



