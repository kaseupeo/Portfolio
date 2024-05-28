using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        _sceneType = Define.SceneType.TitleScene;
    }

    private void Update()
    {
        
    }

    public override void Clear()
    {
    }
   
}
