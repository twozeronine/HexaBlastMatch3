using System.Collections;
using System.Collections.Generic;
using Data;
using UniRx;
using UnityEngine;

public class GameTopUIModel : BaseMVP.IModel
{
    
    public ReactiveProperty<int> Score { get; set; } = new ReactiveProperty<int>();
    public ReactiveProperty<int> MoveCount { get; set; } = new ReactiveProperty<int>();
    public ReactiveProperty<int> MissionCount { get; set; } = new ReactiveProperty<int>();
    public ReactiveProperty<string> MissionType { get; set; } = new ReactiveProperty<string>();

    public StageTableData BeginStageTableData { get; set; }
    
    public void Init()
    {
        Managers.Game.GameTopUIModel = this;
        
        // 스테이지 정보 불러옴
        // 무브 전체 횟수 , 미션 , 미션 갯수 등

        var stageInfo = Managers.Stage.GetStage(21);

        MoveCount.Value = stageInfo.StageTableData.MoveCount;
        MissionCount.Value = stageInfo.StageTableData.MissionCount;
        MissionType.Value = stageInfo.StageTableData.MissionType;

        BeginStageTableData = stageInfo.StageTableData;
    }
}
