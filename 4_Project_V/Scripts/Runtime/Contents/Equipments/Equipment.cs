using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Equipment : SO_Item
{
    public delegate void LevelChangedHandler(Equipment equipment, int currentLevel, int prevLevel);

    public delegate void ActivatedHandler(Equipment equipment);

    public delegate void DeactivatedHandler(Equipment equipment);

    public event LevelChangedHandler OnLevelChanged;
    public event ActivatedHandler OnActivated;
    public event DeactivatedHandler OnDeactivated;

    
    [SerializeField] private Define.ItemType itemType = Define.ItemType.Equipment;
    [SerializeField] private EquipmentType equipType;
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private int weaponID;

    [SerializeReference, SubclassSelector, Tooltip("무기 사용조건")]
    private EquipmentCondition[] useConditions;

    [SerializeField] private bool isAllowLevelExceedData;
    [SerializeField] private int maxLevel;
    [SerializeField, Min(1)] private int defaultLevel = 1;
    [SerializeField] private EquipmentData[] equipmentData;

    

    private EquipmentData _currentData;
    private int _level;

    private GameObject _equipmentObject;
    
    public Creature Owner { get; private set; }
    public Define.ItemType ItemType => itemType;
    public WeaponType WeaponType => weaponType;
    public EquipmentType EquipType => equipType;
    public int WeaponID => weaponID;
    

    public GameObject Prefab => _currentData.prefab;
    public Transform Position => Owner.GetTransformSocket(_currentData.position);
    
    public IReadOnlyList<CreatureCondition> LevelUpConditions => _currentData.levelUpConditions;
    public IReadOnlyList<Cost> LevelUpCosts => _currentData.levelUpCosts;
    public IReadOnlyList<EquipmentCondition> UseConditions => useConditions;
    public int MaxLevel => maxLevel;

    public int Level
    {
        get => _level;
        set
        {
            Debug.Assert(value >= 1 && value <= MaxLevel,
                $"Skill.Rank = {value} - value는 1과 MaxLevel({MaxLevel}) 사이 값이여야합니다.");

            if (_level == value)
                return;

            int prevLevel = _level;
            _level = value;

            var newData = equipmentData.Last(x => x.level <= _level);

            if (newData.level != _currentData.level)
                ChangeData(newData);

            OnLevelChanged?.Invoke(this, _level, prevLevel);
        }
    }

    public int DataBonusLevel => Mathf.Max(_level - _currentData.level, 0);
    public bool IsMaxLevel => _level == maxLevel;

    public bool IsCanLevelUp => !IsMaxLevel && LevelUpConditions.All(x => x.IsPass(Owner)) &&
                                LevelUpCosts.All(x => x.HasEnoughCost(Owner));

    public bool IsActivated { get; private set; }

    public override string Description
    {
        get
        {
            string description = base.Description;

            var stringsByKeyword = new Dictionary<string, string>()
            {
            };

            description = TextReplacer.Replace(description, stringsByKeyword);

            return description;
        }
    }

    public void Init(Creature owner, int level)
    {
        Debug.Assert(owner != null, $"EquipmentItem::SetItem - Owner는 Null이 될 수 없습니다.");
        Debug.Assert(level >= 1 && level <= maxLevel, $"EquipmentItem::SetItem - {level}이 1보다 작거나 {maxLevel}보다 큽니다.");
        Debug.Assert(Owner == null, $"EquipmentItem::SetItem - 이미 Init하였습니다.");

        Owner = owner;
        Level = level;
        
    }

    public void Init(Creature owner)
        => Init(owner, defaultLevel);

    private void ChangeData(EquipmentData newData)
    {
        _currentData = newData;
    }

    public void LevelUp()
    {
        Debug.Assert(IsCanLevelUp, "EquipmentItem::LevelUP - Level Up 조건을 충족하지 못했습니다.");

        foreach (var cost in LevelUpCosts)
            cost.UseCost(Owner);

        Level++;
    }

    public void Activate()
    {
        Debug.Assert(!IsActivated, "EquipmentItem::Activate - 이미 활성화되어 있습니다.");

        GameObject go = Managers.Resource.Instantiate(Prefab, Position, true);
        go.transform.localPosition = Prefab.transform.position;
        go.transform.localRotation = Prefab.transform.rotation;
        _equipmentObject = go;
        
        ApplyStat();
        
        IsActivated = true;
        OnActivated?.Invoke(this);
    }

    public void Deactivate()
    {
        Debug.Assert(IsActivated, "EquipmentItem::Activate - equipment이 활성화되어있지 않습니다.");

        Managers.Pool.Push(_equipmentObject);
        
        CancelStat();
        
        IsActivated = false;
        OnDeactivated?.Invoke(this);
    }

    public void ApplyStat()
    {
        foreach (var stat in _currentData.stats) 
            stat.Apply(this, Owner);
        
        Owner.Stats.ChangedStat();
    }

    public void CancelStat()
    {
        foreach (var stat in _currentData.stats)
        {
            stat.Cancel(this, Owner);
        }
    }
    
    public override object Clone()
    {
        var clone = Instantiate(this);

        if (Owner != null)
            clone.Init(Owner, _level);

        return clone;
    }
}