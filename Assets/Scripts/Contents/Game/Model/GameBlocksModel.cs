using System.Collections;
using System.Collections.Generic;
using Data;
using UniRx;
using UnityEngine;

public class GameBlocksModel : BaseMVP.IModel
{
    public readonly ReactiveProperty<Blocks> BlocksProperty = new ReactiveProperty<Blocks>();
    public void Init()
    {
        Managers.Game.GameBlocksModel = this;

        var initStageBlocks = HexaBlastEngine.InitBlock(Managers.Game.GameTilesModel.TilesProperty.Value);

        BlocksProperty.Value = initStageBlocks;
    }
}
