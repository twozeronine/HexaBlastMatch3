using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GameBlocksPresenter : BaseMVP.IPresenter
{
    private readonly GameBlocksView gameBlocksView;
    private GameBlocksModel gameBlocksModel;
    
    public GameBlocksPresenter(BaseMVP.IView gameView)
    {
        gameBlocksView = gameView as GameBlocksView;
    }

    public void LoadData()
    {
        if(gameBlocksModel == null) return;
        
        gameBlocksModel.Init();
    }

    public void CreateModel()
    {
        gameBlocksModel = new GameBlocksModel();
        
        // Set Observe

        if (gameBlocksModel.BlocksProperty.HasValue)
            gameBlocksModel.BlocksProperty.AsObservable()
                .Subscribe(newBlockMap =>
                {
                    gameBlocksView.SetLayout(newBlockMap);
                }).AddTo(gameBlocksView);
    }
}
