using System;
using UnityEngine;

[Serializable]
public class EquipmentReward : Reward
{
    [SerializeField] private Equipment equipment;

    public override Sprite Icon => equipment.Icon;

    public override void Give(Quest quest)
    {
        // TODO : TEST
        // var gainEquipment = Managers.Game.Player.EquipmentSystem.GainEquipment(equipment);

        var clone = equipment.Clone() as Equipment;
        clone.Init(Managers.Game.Player);
        Managers.Game.Inventory.AddItem(Managers.Data.SetItemData(clone, Quantity));
    }

    public override object Clone() => new EquipmentReward { equipment = equipment };
}