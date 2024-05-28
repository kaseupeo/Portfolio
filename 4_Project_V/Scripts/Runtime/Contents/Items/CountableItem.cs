using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CountableItem : Item
{
    [SerializeField] public SO_CountableItem CountableItemData { get; private set; }
    private int amount;
    public int Amount => amount;
    public bool IsMax => amount >= CountableItemData.MaxAmount;
    public bool IsEmpty => amount <= 0;


    public CountableItem(SO_CountableItem itemData, int amount = 1) : base(itemData)
    {
        this.CountableItemData = itemData;
        SetAmount(amount);
    }

    public override bool Use()
    {
        for (int i = 0; i < CountableItemData.Effect.Length; i++)
        {
            if (Managers.Game.Inventory.Owner == null) 
                Managers.Game.Inventory.Owner = Managers.Game.Player;
            
            CountableItemData.Effect[i].Apply(this, Managers.Game.Inventory.Owner);

            Managers.Game.Inventory.SubItem(this, 1);
            if (i+1 == CountableItemData.Effect.Length) return true;
        }
        return false;
    }

    private void SetAmount(int amount)
    {
        this.amount = Mathf.Clamp(amount, 0, CountableItemData.MaxAmount);
    }

    public void AddAmount(int value)
    {
        amount += value;
    }
   
    public int AddAmountAndGetExcess(int amount)
    {
        int nextAmount = Amount + amount;
        SetAmount(nextAmount);

        return (nextAmount > CountableItemData.MaxAmount) ? (nextAmount - CountableItemData.MaxAmount) : 0;
    }

    public bool SubAmount(int value)
    {
        if (amount < value && IsEmpty)
        {
            return false;
        }
        
        amount -= value;
        return true;
    }
    
    public CountableItem SeperateAndClone(int value)
    {
       
        if (amount <= 1) return null;

        if (value > amount - 1)
            value = amount - 1;

        amount -= value;
        return new CountableItem(this.CountableItemData, amount);
    }

    public override Item Clone()
    {
        return new CountableItem(this.CountableItemData);
    }
}
