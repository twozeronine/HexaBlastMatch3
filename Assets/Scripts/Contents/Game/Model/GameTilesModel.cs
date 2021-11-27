using System.Collections;
using System.Collections.Generic;
using Data;
using Logic;
using UnityEngine;
using UniRx;

public class GameTilesModel : BaseMVP.IModel
{
    public readonly ReactiveProperty<Tiles> TilesProperty = new ReactiveProperty<Tiles>();
    public void Init()
    {
        Managers.Game.GameTilesModel = this;

        var stageInfo = Managers.Stage.GetStage(21);
        
        TilesProperty.Value = new Tiles() { MaxHeight = 9, MaxWidth = 9, TilesMap = new Dictionary<Vector2Int, Tile>(stageInfo.StageMap)};
    }
}
