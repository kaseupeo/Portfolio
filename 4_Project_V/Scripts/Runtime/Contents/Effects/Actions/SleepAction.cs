using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SleepAction : EffectAction
{
    [SerializeField]
    private Category removeTargetCategory;
    [SerializeField]
    private Category dotCategory;

    private Effect _effect;

    public override void Start(Effect effect, Creature user, Creature target, int level, float scale)
    {
        _effect = effect;
        target.OnTakeDamage += OnTakeDamage;
    }

    public override bool Apply(Effect effect, Creature user, Creature target, int level, int stack, float scale)
    {
        target.SkillSystem.RemoveEffectAll(x => x != effect && x.HasCategory(removeTargetCategory));
        target.StateMachine.ExecuteCommand(CreatureStateCommand.ToSleepingState);
        return true;
    }

    public override void Release(Effect effect, Creature user, Creature target, int level, float scale)
    {
        target.OnTakeDamage -= OnTakeDamage;
        target.StateMachine.ExecuteCommand(CreatureStateCommand.ToDefaultState);
    }

    public override object Clone()
    {
        return new SleepAction()
        {
            removeTargetCategory = removeTargetCategory,
            dotCategory = dotCategory
        };
    }

    private void OnTakeDamage(Creature creature, Creature instigator, object causer, float damage)
    {
        var causerEffect = causer as Effect;
        if (causerEffect && causerEffect.HasCategory(dotCategory))
            return;

        creature.SkillSystem.RemoveEffect(_effect);
    }
}
