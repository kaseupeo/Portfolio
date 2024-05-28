using System;
using UnityEngine;

[Serializable]
public class ItemReward : Reward
{
    [SerializeField] private SO_Item item;

    public override Sprite Icon => item.Icon;

    public override void Give(Quest quest)
    {
        Managers.Game.Inventory.AddItem(Managers.Data.SetItemData(item, Quantity));
    }

    public override object Clone() => new ItemReward() { item = item };
}