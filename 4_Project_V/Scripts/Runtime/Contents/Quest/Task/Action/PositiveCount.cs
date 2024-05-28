using System;

[Serializable]
public class PositiveCount : TaskAction
{
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return successCount > 0 ? currentSuccess + successCount : currentSuccess;
    }
    
    public override object Clone() => new PositiveCount();
}