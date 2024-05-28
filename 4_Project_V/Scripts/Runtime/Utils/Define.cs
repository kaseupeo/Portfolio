public class Define
{
    public const float MinSceneLoadingTime = 5f;


    // Build Settings - Scenes In Build 순서맞춰서
    public enum SceneType
    {
        TitleScene,
        LobbyScene,
        LoadingScene,
        GameScene,
    }

    #region Game System

    public enum ScreenMode
    {
        FullScreen,
        Windowed,
    }

    public enum GameSpeed
    {
        Pause = 0,
        Speed1X = 1,
        Speed2X = 2,
        Speed3X = 3,
    }

    #endregion

    public enum CreatureControlType
    {
        Player,
        AI,
    }

    #region 효과 Enum

    public enum EffectType
    {
        None,
        Buff,       // 버프
        DeBuff,     // 디버프
    }

    public enum EffectRemoveDuplicateTargetOption
    {
        Old,        // 이미 적용중인 효과 제거
        New,        // 새로 적용된 효과 제거
    }

    public enum EffectRunningFinishOption
    {
        // Effect가 설정된 적용 횟수만큼 적용된다면 완료되는 Option.
        // 단, 이 Option은 지속 시간(=Duration)이 끝나도 완료됨.
        // 타격을 입힌다던가, 치료를 해주는 Effect에 적합Option
        FinishWhenApplyCompleted,
        // 지속 시간이 끝나면 완료되는 Option.
        // Effect가 설정된 적용 횟수만큼 적용되도, 지속 시간이 남았다면 완료가 안됨.
        // 처음 한번 적용되고, 일정 시간동안 지속되는 Buff나 Debuff Effect에 적합한 Option.
        FinishWhenDurationEnded,
    }

    #endregion

    public enum SearchResultMessage
    {
        Fail,               // 기준점을 찾지 못함
        OutOfRange,         // 기준점이 존재하지만 검색 범위 밖에 있음
        FindTarget,         // 기준 Target을 찾음
        FindPosition,       // 기준 Position을 찾음
    }

    public enum Direction
    {
        Forward,
        Backward,
        Right,
        Left,
    }

    public enum Rarity
    {
        Normal,
        Rare,
        Unique,
        Myth,
    }

    public enum ItemType
    {
        Equipment,
        Consume,
        Etc,
    }


    // UI 창들
    public enum InterfaceElements
    {
        Map,
        Inventory,
        Quest,
        Skill,
        Stat,
        MainMenu,
    }

    #region 퀘스트

    public enum QuestState
    {
        Inactive,
        Running,
        Complete,
        Cancel,
        WaitingForCompletion,
    }

    public enum TaskState
    {
        Inactive,
        Running,
        Complete,
    }

    public enum TaskGroupState
    {
        Inactive,
        Running,
        Complete,
    }

    #endregion
}