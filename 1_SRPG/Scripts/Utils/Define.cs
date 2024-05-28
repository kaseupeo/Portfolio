using UnityEngine;

public class Define
{
    // 크리처가 이동할 수 있는 높이
    public const int Height = 1;
    
    public enum GameMode
    {
        Preparation,
        PlayerTurn,
        MonsterTurn,
    }
    
    public enum GameSpeedMode
    {
        Slow,
        Normal,
        Fast,
    }
    
    public enum CameraMode
    {
        Both,
        Mouse,
        Keyboard,
    }
    
    public enum State
    {
        Idle,
        Move,
        Attack,
        Dead,
        Inventory,
    }
    
    public enum SceneType
    {
        LobbyScene,
        GameScene,
    }

    public enum ArrowDirection
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        TopRight = 5,
        BottomRight = 6,
        TopLeft = 7,
        BottomLeft = 8,
        UpFinished = 9,
        DownFinished = 10,
        LeftFinished = 11,
        RightFinished = 12
    }
}