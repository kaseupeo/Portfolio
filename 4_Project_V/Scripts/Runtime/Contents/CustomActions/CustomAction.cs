using System;

[Serializable]
public abstract class CustomAction : ICloneable
{
    public virtual void Start(object data) { }
    public virtual void Run(object data) { }
    public virtual void Release(object data) { }
    
    public abstract object Clone();
}