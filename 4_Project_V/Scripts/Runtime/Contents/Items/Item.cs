using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : IUsable
{
    
    [SerializeField] private SO_Item itemData;
    private string itemName => itemData.DisplayName;
    public SO_Item ItemData => itemData;
    public Define.ItemType Type => itemData.Type;
    public string ItemName => itemName;

    public Sprite Icon => itemData.Icon;

    public Item(SO_Item itemData)
    {
        this.itemData = itemData;
    }

    public virtual bool Use()
    {
        return false;
    }

    public abstract Item Clone();
}

