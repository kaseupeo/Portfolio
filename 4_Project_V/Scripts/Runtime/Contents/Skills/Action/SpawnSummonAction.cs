using System;
using UnityEngine;

[Serializable]
public class SpawnSummonAction : SkillAction
{
    [SerializeField] private GameObject summonPrefab;
    [SerializeField] private Skill[] skills;
    
    public override void Apply(Skill skill)
    {
        SummonController summon = GameObject.Instantiate(summonPrefab).GetComponent<SummonController>();
        summon.transform.position = skill.Owner.transform.position + skill.Owner.transform.forward; 
        summon.Init(skill.Owner, skill, skills);
    }

    public override object Clone()
    {
        return new SpawnSummonAction()
        {
            summonPrefab = summonPrefab,
            skills = skills
        };
    }
}