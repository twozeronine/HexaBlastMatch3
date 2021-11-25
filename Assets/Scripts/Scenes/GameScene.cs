using System.Collections;
using System.Collections.Generic;
using Logic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Scene.Game;
    }
    
    
    public override void Clear()
    {
        // 해당 씬에서 비워줘야 할 것이 있을시에 작성
    }
}
