using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        var completeBuildStageInitBlocks = false;

        var count = 0;
        while (!completeBuildStageInitBlocks)
        {
            var match3BlockList = HexaBlastEngine.ScanInitMatched3Blocks(initStageBlocks);

            if (match3BlockList.Any())
            {
                HexaBlastEngine.ShuffledMatched3Blocks(ref initStageBlocks,match3BlockList);
            }
            else
            {
                completeBuildStageInitBlocks = true;    
            }
            Debug.Log(count+"번 섞음");
            count++;
        }
        
        BlocksProperty.Value = initStageBlocks;
    }
}
