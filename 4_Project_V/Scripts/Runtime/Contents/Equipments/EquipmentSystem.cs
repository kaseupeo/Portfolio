using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EquipmentSystem : MonoBehaviour
{
    public delegate void WeaponChangedHandler(EquipmentSystem equipmentSystem, Equipment equipment);
    public event WeaponChangedHandler OnWeaponChanged;
    
    // 기본 장비
    [SerializeField] private Equipment[] defaultEquipments;

    private List<Equipment> _ownEquipmentList = new();
    private List<Equipment> _ownWeaponList = new();
    private List<Equipment> _ownArmorList = new();

    private List<Equipment> _registeredWeaponList = new();
    // private Queue<Equipment> _registeredWeaponQueue = new();

    private GameObject _equipWeaponObject;
    
    public Creature Owner { get; private set; }

    public IReadOnlyList<Equipment> OwnEquipmentList => _ownEquipmentList;
    public IReadOnlyList<Equipment> OwnWeaponList => _ownWeaponList;
    public IReadOnlyList<Equipment> OwnArmorList => _ownArmorList;

    public Equipment EquipWeapon => _registeredWeaponList[0];

    public void Init(Creature creature)
    {
        Owner = creature;
        
        foreach (Equipment equipment in defaultEquipments)
        {
            GainEquipment(equipment);
            Managers.Game.Inventory.Equip((EquipmentItem)Managers.Data.SetItemData(equipment));
        }

        Equip(EquipWeapon);
        Managers.Game.OnWeaponEquiped += RegisterWeapon;
    }

    public Equipment GainEquipment(Equipment equipment, int level = 0)
    {
        var clone = equipment.Clone() as Equipment;

        if (level > 0)
            clone.Init(Owner, level);
        else
            clone.Init(Owner);

        _ownEquipmentList.Add(clone);

        switch (clone.EquipType)
        {
            case EquipmentType.Weapon:
                _ownWeaponList.Add(clone);
                if (_registeredWeaponList.Count < 3)
                    RegisterWeapon(clone);
                break;
            case EquipmentType.Armor:
                _ownArmorList.Add(clone);
                break;
        }

        Managers.Game.Inventory.AddItem(Managers.Data.SetItemData(equipment));
        return clone;
    }

    public void ExchangedWeapon(Equipment equipment)
    {
        var find = _registeredWeaponList.Find(x => x.WeaponType == equipment.WeaponType);
        if (find.IsActivated) 
            find.Deactivate();
        
        _registeredWeaponList.Remove(find);
        _registeredWeaponList.Add(equipment);
        if (equipment.Owner == null) 
            equipment.Init(Owner);
    }
    
    public void RegisterWeapon(Equipment equipment)
    {
        if (_ownWeaponList.Find(x => x == equipment)) 
            _registeredWeaponList.Add(equipment);
    }

    public void UnregisterWeapon(Equipment equipment)
    {
        int count = _registeredWeaponList.Count;
        
        for (int i = 0; i < count; i++)
        {
            var dequeue = _registeredWeaponList[0];
            _registeredWeaponList.RemoveAt(0);
            if (dequeue == equipment)
                continue;
            
            _registeredWeaponList.Add(dequeue);
        }
    }

    public void Equip(Equipment equipment)
    {
        if (equipment == null)
            return;

        if (!equipment.IsActivated)
            equipment.Activate();
    }
    
    public void ChangeEquipWeapon()
    {
        if (_registeredWeaponList.Count <= 1)
            return;

        var prevEquipment = _registeredWeaponList[0];
        _registeredWeaponList.RemoveAt(0);
        
        if (prevEquipment.IsActivated) 
            prevEquipment.Deactivate();
        
        _registeredWeaponList.Add(prevEquipment);
        Equip(EquipWeapon);
        
        OnWeaponChanged?.Invoke(this, EquipWeapon);
    }
}