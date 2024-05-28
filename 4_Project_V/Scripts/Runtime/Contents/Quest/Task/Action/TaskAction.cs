using System;
using UnityEngine;

[Serializable]
public abstract class TaskAction : ICloneable
{
    public abstract int Run(Task task, int currentSuccess, int successCount);

    public abstract object Clone();
}