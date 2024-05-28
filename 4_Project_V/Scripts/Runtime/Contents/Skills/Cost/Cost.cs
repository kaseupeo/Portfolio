using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Cost : ICloneable
{
    public abstract string Description { get; }

    public abstract bool HasEnoughCost(Creature creature);
    public abstract void UseCost(Creature creature);
    public abstract void UseDeltaCost(Creature creature);
    public abstract float GetValue(Creature creature);
    public abstract object Clone();
}
