using System;
using UnityEngine;

[Serializable]
public struct EffectData
{
    public int level;

    [UnderlineTitle("Stack")] 
    [Min(1), Tooltip("효과가 중첩될 수 있는 최대 스택")] 
    public int maxStack;

    [Tooltip("스택에 따른 추가 효과들")]
    public EffectStackAction[] stackActions;

    [UnderlineTitle("Action")] 
    [SerializeReference, SubclassSelector, Tooltip("효과들을 실제로 구현하는 모듈")] 
    public EffectAction action;

    [UnderlineTitle("Setting")]
    [Tooltip("효과를 완료(종료)할 시점")]
    public Define.EffectRunningFinishOption runningFinishOption;
    [Tooltip("효과의 지속 시간이 만료되었을떄, 남은 적용 횟수가 있다면 모두 적용할 것인지 확인하는 여부")]
    public bool isApplyAllWhenDurationExpires;
    [Tooltip("효과의 지속시간")]
    public StatScaleFloat duration;
    [Min(0), Tooltip("효과를 적용할 횟수")]
    public int applyCount;
    [Min(0f), Tooltip("효과를 적용할 주기")]
    public float applyCycle;

    [UnderlineTitle("Custom Action")] 
    [SerializeReference, SubclassSelector, Tooltip("효과에 다양한 연출(카메라, 사운드, 파티클 등)을 주기위한 모듈")]
    public CustomAction[] customActions;
}