using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPotion : Potion
{
    [SerializeField] private int healthPoint;
    
    public override void Use()
    {
        Managers.Game.SelectedCharacter.Healing(healthPoint);
        base.Use();
    }
}
