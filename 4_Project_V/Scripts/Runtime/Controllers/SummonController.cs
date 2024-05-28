using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SummonController : BaseController
{
    [SerializeField, ReadOnly] private Creature target;

    private Creature _creature;
    private Creature _owner;
    private Skill _skill;
    private IReadOnlyList<Skill> _skillList;

    private void Update()
    {
        target = _creature.Target;

        if (_creature.IsDead)
            return;
        
        Attack();
    }

    public void Init(Creature owner, Skill skill, Skill[] skills)
    {
        _creature = GetComponent<Creature>();
        _creature.Target = _creature;
        _creature.OnHandOverExp += GainExp;
        _creature.OnDead += Dead;

        _owner = owner;
        _skill = skill.Clone() as Skill;
        
        foreach (Skill summonSkill in skills) 
            _creature.SkillSystem.Register(summonSkill);

        _skillList = _creature.SkillSystem.OwnSkillList;
    }
    
    private void Attack()
    {
        if (_skillList[0].IsReady && _creature != _creature.Target)
        {
            _creature.Movement.TraceTarget = _creature.Target.transform;

            if (Vector3.Distance(transform.position, target.transform.position) < (float)_skillList[0].TargetSearcher.SelectionRange)
                _skillList[0].Use();
        }
    }

    private void GainExp(Creature creature, float exp)
    {
        _owner.GainExp(exp);
    }
    
    private void Dead(Creature creature)
    {
        Destroy(_skill);
        Destroy(gameObject, 1f);
    }
}