using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public  class MonsterSkillState : SkillState
{
    // ���� Entity�� �������� Skill
    public Skill RunningSkill { get; private set; }
    // Entity�� �����ؾ��� Animation�� Hash
    protected int AnimatorParameterHash { get; private set; }

    public MonsterController controller;
    public Creature creature;

    protected override void Init()
    {
       
    }

    public override void Enter()
    {

        //Creature.Movement?.Stop();

        //if (monsterController)
        //    monsterController.enabled = false;

    }

    public override void Exit()
    {

    }
    

}
