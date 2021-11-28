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

        Blocks initStageBlocks;
        var isFinishInitBlocks = false;

        var escapeCount = 0; // 로직에 오류가 있을 시 비상 탈출 카운트
        do
        {
            initStageBlocks = HexaBlastEngine.InitBlock(Managers.Game.GameTilesModel.TilesProperty.Value);

            var completeBuildStageInitBlocks = false;

            // 초기 설정된 블럭중 매치 3 가 된 블럭이 있는지 검사하고 있으면 섞어 주는 로직
            var count = 0;
            while (!completeBuildStageInitBlocks)
            {
                var match3BlockList = HexaBlastEngine.ScanInitMatched3Blocks(initStageBlocks);

                if (match3BlockList.Any())
                {
                    HexaBlastEngine.ShuffledMatched3Blocks(ref initStageBlocks, match3BlockList);
                }
                else
                {
                    completeBuildStageInitBlocks = true;
                }

                Debug.Log(count + "번 섞음");
                count++;
            }

            // 섞어서 나온 블럭에서 더이상 제거 할 블럭이 없는지 검사하는 로직
            isFinishInitBlocks = HexaBlastEngine.TryCheckCanRemoveBlocks(initStageBlocks, out var hintBlock);
            Debug.Log("탈출 :" + isFinishInitBlocks);
            escapeCount++;
            // 로직에 오류가 있을 시 비상 탈출문
            if (escapeCount == 100) break;
        } while (!isFinishInitBlocks);

        foreach (var tilesMapValue in Managers.Game.GameTilesModel.TilesProperty.Value.TilesMap.Values)
        {
            tilesMapValue.ChildBlock = initStageBlocks.GetBlock(tilesMapValue.TilePos);
        }
        
        BlocksProperty.Value = initStageBlocks;
    }
}
