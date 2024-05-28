using UnityEngine;

public class LoadingScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        _sceneType = Define.SceneType.LoadingScene;
    }

    private void Start()
    {
        StartCoroutine(Managers.Scene.LoadSceneAsync(Managers.Scene.NextScene));
    }
    
    public override void Clear()
    {
    }
}