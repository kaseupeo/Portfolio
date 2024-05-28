using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class CreatureCondition : Condition<Creature>
{
    public abstract string Description { get; }
}
