using System;

[Serializable]
public class ContinuousCount : TaskAction
{
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return successCount > 0 ? currentSuccess + successCount : 0;
    }

    public override object Clone() => new ContinuousCount();
}