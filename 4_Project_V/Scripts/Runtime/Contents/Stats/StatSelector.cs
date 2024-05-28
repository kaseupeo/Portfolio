using System;
using UnityEngine;

[Serializable]
public struct StatSelector
{
    public Stat stat;
    public float value;

    private float _totalValue;
    
    public float GetValue(Stats stats) => stat && stats.HasStat(stat) ? value : 0;
    
    public bool Apply(Equipment equipment, Creature user)
    {
        user.Stats.SetBonusValue(stat, equipment, value);

        return true;
    }

    public bool Cancel(Equipment equipment, Creature user)
    {
        user.Stats.RemoveBonusValue(stat, equipment);

        return true;
    }
}