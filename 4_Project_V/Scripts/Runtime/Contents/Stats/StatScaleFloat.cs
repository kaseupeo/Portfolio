using System;

[Serializable]
public struct StatScaleFloat
{
    public float defaultValue;
    public Stat scaleStat;

    public float GetValue(Stats stats)
    {
        return scaleStat && stats.TryGetStat(scaleStat, out var stat) 
            ? defaultValue * (1 + stat.Value) 
            : defaultValue;
    }
}