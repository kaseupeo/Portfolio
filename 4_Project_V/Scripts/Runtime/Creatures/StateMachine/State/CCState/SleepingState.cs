using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepingState : CreatureCCState
{
    private static readonly int SleepingHash = Animator.StringToHash("IsSleeping");

    public override string Description => "수면";
    protected override int AnimationHash => SleepingHash;
}
