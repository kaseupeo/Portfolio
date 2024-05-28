using UnityEngine.Serialization;

public struct QuestSaveData
{
    public string CodeName;
    public Define.QuestState State;
    public int TaskGroupIndex;
    public int[] TaskSuccessCounts;
}