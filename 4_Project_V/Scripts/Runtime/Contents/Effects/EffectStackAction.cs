using System;
using UnityEngine;

[Serializable]
public class EffectStackAction
{
    [SerializeField, Min(1)] private int stack;
    [SerializeField] private bool isReleaseOnNextApply;
    [SerializeField] private bool isApplyOnceInLifeTime;

    [UnderlineTitle("Action")] 
    [SerializeReference, SubclassSelector]
    private EffectAction action;

    private bool _hasEverApplied;

    public int Stack => stack;
    public bool IsReleaseOnNextApply => isReleaseOnNextApply;
    public bool IsApplicable => !isApplyOnceInLifeTime || (isApplyOnceInLifeTime && !_hasEverApplied);

    public void Start(Effect effect, Creature user, Creature target, int level, float scale)
        => action.Start(effect, user, target, level, scale);

    public void Apply(Effect effect, int level, Creature user, Creature target, float scale)
    {
        action.Apply(effect, user, target, level, stack, scale);
        _hasEverApplied = true;
    }

    public void Release(Effect effect, int level, Creature user, Creature target, float scale)
        => action.Release(effect, user, target, level, scale);

    public string BuildDescription(Effect effect, string baseDescription, int stackActionIndex, int effectIndex)
        => action.BuildDescription(effect, baseDescription, stackActionIndex, stack, effectIndex);
}