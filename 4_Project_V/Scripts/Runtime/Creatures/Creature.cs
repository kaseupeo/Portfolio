using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public delegate void GainExpHandler(Creature creature, float exp);
    public delegate void LevelUpHandler(Creature creature, float level);
    public delegate void GainGoldHandler(Creature creature, float gold);
    public delegate void TakeDamageHandler(Creature creature, Creature instigator, object causer, float damage);
    public delegate void DeadHandler(Creature creature);

    public event GainExpHandler OnGainExp;
    public event GainExpHandler OnHandOverExp;
    public event LevelUpHandler OnLevelUp;
    public event GainGoldHandler OnGainGold;
    public event TakeDamageHandler OnTakeDamage;
    public event DeadHandler OnDead;
    
    // 아군, 적군 등을 식별하기 위한 카테고리
    [SerializeField] private Category[] categories;
    [SerializeField] private Define.CreatureControlType controlType;
    
    private Dictionary<string, Transform> _socketsByNameDic = new Dictionary<string, Transform>();

    public Animator Animator { get; private set; }
    public Stats Stats { get; private set; }
    public CreatureMovement Movement { get; private set; }
    public CreatureSubMovement SubMovement { get; private set; }
    public Creature Target { get; set; }
    public MonoStateMachine<Creature> StateMachine { get; private set; }
    public SkillSystem SkillSystem { get; private set; }
    public EquipmentSystem EquipmentSystem { get; private set; }
    public IReadOnlyList<Category> Categories => categories;
    public Define.CreatureControlType ControlType => controlType;
    public bool IsDead => Stats.HPStat != null && Mathf.Approximately(Stats.HPStat.DefaultValue, 0f);
    public bool IsPlayer => ControlType == Define.CreatureControlType.Player;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        
        Stats = GetComponent<Stats>();
        Stats.Init(this);

        Movement = GetComponent<CreatureMovement>();
        Movement?.Init(this);
        
        SubMovement = GetComponent<CreatureSubMovement>();
        SubMovement?.Init(this);
        
        StateMachine = GetComponent<MonoStateMachine<Creature>>();
        StateMachine?.Init(this);

        SkillSystem = GetComponent<SkillSystem>();
        SkillSystem?.Init(this);

        EquipmentSystem = GetComponent<EquipmentSystem>();
        EquipmentSystem?.Init(this);
    }

    private void OnDisable()
    {
        OnDead = null;
    }

    public void Init()
    {
        Stats.Init(this);
        Movement.enabled = true;
        SkillSystem.enabled = true;
    }
    
    private Transform GetTransformSocket(Transform root, string socketName)
    {
        if (root.name == socketName)
            return root;

        foreach (Transform child in root)
        {
            var socket = GetTransformSocket(child, socketName);

            if (socket)
                return socket;
        }

        return null;
    }

    public Transform GetTransformSocket(string socketName)
    {
        if (_socketsByNameDic.TryGetValue(socketName, out var socket))
            return socket;

        socket = GetTransformSocket(transform, socketName);

        if (socket)
            _socketsByNameDic[socketName] = socket;

        return socket;
    }

    public void GainExp(float exp)
    {
        if (HasCategory("RELATIONSHIP_SUMMON"))
            OnHandOverExp?.Invoke(this, exp);
        
        if (IsDead || !IsPlayer)
            return;
        
        float prevValue = Stats.ExpStat.DefaultValue;

        if (prevValue + exp >= Stats.ExpStat.MaxValue)
        {
            Stats.ExpStat.DefaultValue = prevValue + exp - Stats.ExpStat.DefaultValue;
            LevelUp();
        }
        else
        {
            Stats.ExpStat.DefaultValue += exp;
        }

        OnGainExp?.Invoke(this, exp);
    }

    // TODO : 레벨 데이터 관리로 빼야함
    public void LevelUp()
    {
        Stats.HPStat.DefaultValue = Stats.HPStat.MaxValue;
        Stats.MPStat.DefaultValue = Stats.MPStat.MaxValue;
        Stats.LevelStat.DefaultValue++;
        Stats.ExpStat.MaxValue += 20;
        Stats.StatPoint.DefaultValue += 5;
        Stats.SkillPoint.DefaultValue += 5;
        OnLevelUp?.Invoke(this, Stats.LevelStat.DefaultValue);
    }

    public void GainGold(float gold)
    {
        Stats.GoldStat.DefaultValue += gold;
        OnGainGold?.Invoke(this, Stats.GoldStat.DefaultValue);
    }
    
    public void TakeDamage(Creature instigator, object causer, float damage)
    {
        if (IsDead)
            return;

        float prevValue = Stats.HPStat.DefaultValue;
        Stats.HPStat.DefaultValue -= damage;

        OnTakeDamage?.Invoke(this, instigator, causer, damage);

        if (Mathf.Approximately(Stats.HPStat.DefaultValue, 0f)) 
            Dead(instigator);
    }

    private void Dead(Creature instigator)
    {
        if (Movement)
            Movement.enabled = false;

        if (SkillSystem)
            SkillSystem.enabled = false;
        
        instigator.GainExp(Stats.ExpStat.DefaultValue);
        OnDead?.Invoke(this);
    }
  
    public bool IsInState<T>() where T : State<Creature>
        => StateMachine.IsInState<T>();

    public bool IsInState<T>(int layer) where T : State<Creature>
        => StateMachine.IsInState<T>(layer);
    
    public bool HasCategory(Category category)
        => categories.Any(x => x.ID == category.ID);
    
    public bool HasCategory(string category) 
        => categories.Any(x => x == category);
}