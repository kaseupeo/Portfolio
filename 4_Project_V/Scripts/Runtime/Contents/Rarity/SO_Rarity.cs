using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rarity = Define.Rarity;

[CreateAssetMenu(fileName = "rarity")]
[Serializable]
public class SO_Rarity : ScriptableObject
{
    [SerializeField] private Rarity itemRarirty;
    [SerializeField] private Sprite rarityicon;
    [SerializeField] private float rate;

    public Rarity ItemRarirty => itemRarirty;
    public Sprite Rarityicon => rarityicon;
    public float Rate => rate;
}
