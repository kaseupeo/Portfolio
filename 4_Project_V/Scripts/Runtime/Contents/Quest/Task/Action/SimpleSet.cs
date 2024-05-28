using System;

[Serializable]
public class SimpleSet : TaskAction
{
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return successCount;
    }

    public override object Clone() => new SimpleSet();
}