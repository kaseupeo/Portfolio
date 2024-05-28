using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunningState : CreatureCCState
{
    private static readonly int StunningHash = Animator.StringToHash("IsStunning");

    public override string Description => "기절";
    protected override int AnimationHash => StunningHash;
}
