using System;

[Serializable]
public class SimpleCount : TaskAction
{
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return currentSuccess + successCount;
    }

    public override object Clone() => new SimpleCount();
}