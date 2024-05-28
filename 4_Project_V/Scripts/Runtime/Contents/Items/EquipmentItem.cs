using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItem : Item
{
    public Skill itemSkill;
    public Equipment SO_equipment;
    public WeaponType weaponType => SO_equipment.WeaponType;
   
    public EquipmentItem(Equipment SO_Data) : base(SO_Data)
    {
        this.SO_equipment = SO_Data;
    }
    public override bool Use()
    {
        return false;
    }

    public override Item Clone()
    {
        return new EquipmentItem(this.SO_equipment);
    }

}

