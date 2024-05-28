using System;
using UnityEngine;
using UnityEngine.UI;

public class ChangeWeaponButtonUI : MonoBehaviour
{
    [SerializeField] private Image weaponIcon;
    
    private Button _button;
    private EquipmentSystem _equipmentSystem;
    
    private void Start()
    {
        _button = GetComponent<Button>();
        
        _equipmentSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<EquipmentSystem>();

        _equipmentSystem.OnWeaponChanged += OnChangedWeaponIcon;
        _button.onClick.AddListener(_equipmentSystem.ChangeEquipWeapon);

        OnChangedWeaponIcon(_equipmentSystem, _equipmentSystem.EquipWeapon);
    }

    private void OnChangedWeaponIcon(EquipmentSystem equipmentSystem, Equipment equipment)
    { 
        weaponIcon.sprite = equipment.Categories[0].Icon;
    }
}