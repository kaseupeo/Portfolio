using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        _sceneType = Define.SceneType.GameScene;
        
        Managers.Quest.Init();
    }
    
    public override void Clear()
    {
    }
}
