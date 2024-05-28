using System;
using UnityEngine;

[Serializable]
public abstract class Reward : ICloneable
{
    [SerializeField] private Sprite icon;
    [SerializeField] private string description;
    [SerializeField] private int quantity;

    public virtual Sprite Icon => icon;
    public string Description => description;
    public int Quantity => quantity;

    public abstract void Give(Quest quest);
    public abstract object Clone();
}