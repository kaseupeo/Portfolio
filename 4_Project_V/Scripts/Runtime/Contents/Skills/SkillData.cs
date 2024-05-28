using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public struct SkillData
{
    public int level;

    // Skill Level Up을 위한 조건
    [UnderlineTitle("Level Up")]
    [SerializeReference, SubclassSelector, Tooltip("Skill Level Up을 위한 조건")]
    public CreatureCondition[] levelUpConditions;
    // Skill Level Up을 위한 비용
    [SerializeReference, SubclassSelector, Tooltip("Skill Level Up을 위한 비용")]
    public Cost[] levelUpCosts;

    // Skill이 실제 사용되기 전 먼저 실행할 Action, 아무 효과 없이 어떤 동작을 수행하기 위해 존재
    // ex. 상대방에게 달려감, 구르기를 함, Jump를 함 등
    [UnderlineTitle("Preceding Action")]
    [SerializeReference, SubclassSelector]
    [Tooltip("Skill이 실제 사용되기 전 먼저 실행할 Action, 아무 효과 없이 어떤 동작을 수행하기 위해 존재")]
    public SkillPrecedingAction precedingAction;

    // Skill의 사용 방식을 담당하는 Module
    // ex. 투사체 발사, Target에게 즉시 적용, Skill Object Spawn 등
    [UnderlineTitle("Action")]
    [SerializeReference, SubclassSelector]
    [Tooltip("Skill의 사용 방식을 담당하는 Module")]
    public SkillAction action;

    [UnderlineTitle("Setting")]
    [Tooltip("스킬을 언제 끝낼지")]
    public SkillRunningFinishOption runningFinishOption;
    // runningFinishOption이 FinishWhenDurationEnded이고 duration이 0이면 무한 지속
    [Min(0), Tooltip("지속 시간")]
    public float duration;
    // applyCount가 0이면 무한 적용
    [Min(0), Tooltip("스킬 발동 횟수")]
    public int applyCount;
    // 첫 한번은 효과가 바로 적용될 것이기 때문에, 한번 적용된 후부터 ApplyCycle에 따라 적용됨
    // 예를 들어서, ApplyCycle이 1초라면, 바로 한번 적용된 후 1초마다 적용되게 됨. 
    [Min(0f), Tooltip("스킬 발동 주기")]
    public float applyCycle;

    public StatScaleFloat cooldown;

    // Skill의 적용 대상을 찾기 위한 Class
    [UnderlineTitle("TargetPlayer Searcher")]
    [Tooltip("Skill을 적용할 Target을 찾기 위한 TargetSearcher")]
    public TargetSearcher targetSearcher;

    // Skill 사용을 위한 비용
    [UnderlineTitle("Cost")]
    [SerializeReference, SubclassSelector, Tooltip("Skill 사용을 위한 비용")]
    public Cost[] costs;

    [UnderlineTitle("Cast")]
    [Tooltip("캐스팅 사용 유무")]
    public bool isUseCast;
    public StatScaleFloat castTime;

    [UnderlineTitle("Charge")]
    [Tooltip("차지 사용 유무")]
    public bool isUseCharge;
    public SkillChargeFinishActionOption chargeFinishActionOption;
    // Charge의 지속 시간
    [Min(0f), Tooltip("Charge의 지속 시간")]
    public float chargeDuration;
    // Full Charge까지 걸리는 시간
    [Min(0f), Tooltip("Full Charge까지 걸리는 시간")]
    public float chargeTime;
    // Skill을 사용하기 위해 필요한 최소 충전 시간
    [Min(0f), Tooltip("Skill을 사용하기 위해 필요한 최소 충전 시간")]
    public float needChargeTimeToUse;
    // Charge의 시작 Power
    [Range(0f, 1f), Tooltip("Charge의 시작 Power")]
    public float startChargePower;
    
    [UnderlineTitle("Effect")]
    public EffectSelector[] effectSelectors;

    [UnderlineTitle("Animation")]
    [Tooltip("Action State를 언제 끝낼지")]
    public InSkillActionFinishOption inSkillActionFinishOption;
    public AnimatorParameter castAnimatorParameter;
    public AnimatorParameter chargeAnimatorParameter;
    public AnimatorParameter precedingActionAnimatorParameter;
    public AnimatorParameter actionAnimatorParameter;

    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnCast;
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnCharge;
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnPrecedingAction;
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnAction;
}
