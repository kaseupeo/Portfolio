using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StunAction : EffectAction
{
    [SerializeField]
    private Category removeTargetCategory;

    public override bool Apply(Effect effect, Creature user, Creature target, int level, int stack, float scale)
    {
        target.SkillSystem.RemoveEffectAll(removeTargetCategory);
        target.StateMachine.ExecuteCommand(CreatureStateCommand.ToStunningState);
        return true;
    }

    public override void Release(Effect effect, Creature user, Creature target, int level, float scale)
        => target.StateMachine.ExecuteCommand(CreatureStateCommand.ToDefaultState);

    public override object Clone() => new StunAction() { removeTargetCategory = removeTargetCategory };
}
