using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class OrcController : CreatureController
{
    [SerializeField] private Skill changedPhaseSkill;
    
    private Creature _creature;
    private Skill _skill;
    private bool _isUseChangedPhaseSkill;
    
    private void Awake()
    {
        _creature = GetComponent<Creature>();
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
        if (_creature.IsDead)
            return;

        if (_creature.Stats.HPStat.Value <= 50 && !_isUseChangedPhaseSkill)
        {
            Skill skill = _creature.SkillSystem.Register(changedPhaseSkill);
            if (skill.Use())
            {
                _isUseChangedPhaseSkill = true;
                _skill = null;
            }
        }

        if (_skill == null)
            _skill = SelectSkill();
        
        Attack(_skill);
    }
    
    private Skill SelectSkill()
    {
        foreach (Skill skill in _creature.SkillSystem.ActiveSkillList)
        {
            if (skill.CodeName == changedPhaseSkill.CodeName)
                continue;
            
            if (skill.IsUseable)
                return skill;
        }

        return null;
    }
    
    private void Attack(Skill skill)
    {
        if (skill == null)
            return;

        if (_creature.Target == null || _creature == _creature.Target)
            return;

        _creature.Movement.TraceTarget = _creature.Target.transform;

        if (Vector3.Distance(transform.position, _creature.Target.transform.position) < (float)skill.TargetSearcher.SelectionRange)
        {
            skill.Use();
            _creature.Target = _creature;
            _creature.Movement.Stop();
            _skill = null;
        }
    }

    private void Dead(Creature creature)
    {
        if (!Managers.Pool.Push(gameObject)) 
            Debug.Log("몬스터 디스폰 에러");
        
        OnDead.Invoke();
    }
}