using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class SlimeController : CreatureController
{
    [SerializeField, ReadOnly] private Creature target;

    private Creature _creature;
    private IReadOnlyList<Skill> _activeSkillList;
    private IReadOnlyList<Skill> _passiveSkillList;

    private void Awake()
    {
        _creature = GetComponent<Creature>();
    }

    private void Start()
    {
        _activeSkillList = _creature.SkillSystem.ActiveSkillList;
        _passiveSkillList = _creature.SkillSystem.PassiveSkillList;
    }

    private void OnEnable()
    {
        _creature.Target = _creature;
        _creature.OnDead += Dead;
    }

    private void OnDisable()
    {
        _creature.OnDead -= Dead;
    }
    
    private void Update()
    {
        target = _creature.Target;

        if (_creature.IsDead)
            return;

        Attack();
    }

    private Skill SelectSkill()
    {
        foreach (Skill skill in _activeSkillList)
        {
            if (skill.IsUseable)
                return skill;
        }

        return null;
    }
    
    private void Attack()
    {
        var selectSkill = SelectSkill();
        
        if (selectSkill == null)
            return;

        if (_creature.Target != null && _creature != _creature.Target)
        {
            _creature.Movement.TraceTarget = _creature.Target.transform;

            if (Vector3.Distance(transform.position, target.transform.position) < (float)selectSkill.TargetSearcher.SelectionRange)
                selectSkill.Use();
        }
    }

    private void Dead(Creature creature)
    {
        if (!Managers.Pool.Push(gameObject)) 
            Debug.Log("몬스터 디스폰 에러");
        
        OnDead?.Invoke();
    }
}