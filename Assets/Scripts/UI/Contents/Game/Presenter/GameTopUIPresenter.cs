using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameTopUIPresenter : BaseMVP.IPresenter
{
    private readonly GameTopUIView gameTopUIView;
    private GameTopUIModel gameTopUIModel;

    public GameTopUIPresenter(BaseMVP.IView topUIView)
    {
        gameTopUIView = topUIView as GameTopUIView;
    }
    
    public void LoadData()
    {
        if(gameTopUIModel == null) return;
        
        gameTopUIModel.Init();
        
        gameTopUIView.SetLayout(gameTopUIModel.BeginStageTableData);
    }

    public void CreateModel()
    {
        gameTopUIModel = new GameTopUIModel();
        
        // SetObserve

        if (gameTopUIModel.MissionCount.HasValue)
            gameTopUIModel.MissionCount.AsObservable().Subscribe(count =>
            {
                gameTopUIView.MissionCountText.text = $"{count}";
            }).AddTo(gameTopUIView);
        
        if(gameTopUIModel.MoveCount.HasValue)
            gameTopUIModel.MoveCount.AsObservable().Subscribe(count =>
            {
                gameTopUIView.MoveCountText.text = $"{count}";
            }).AddTo(gameTopUIView);
        
        if(gameTopUIModel.Score.HasValue)
            gameTopUIModel.Score.AsObservable().Subscribe(count =>
            {
                gameTopUIView.SetScore(count);
            }).AddTo(gameTopUIView);
    }
}
