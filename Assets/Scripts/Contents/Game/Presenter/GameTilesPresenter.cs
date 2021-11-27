using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameTilesPresenter : BaseMVP.IPresenter
{
    private readonly GameTilesView gameTilesView;
    private GameTilesModel gameTilesModel;

    public GameTilesPresenter(BaseMVP.IView gameView)
    {
        gameTilesView = gameView as GameTilesView;
    }
    
    public void LoadData()
    {
        gameTilesModel?.Init();
    }

    public void CreateModel()
    {
        gameTilesModel = new GameTilesModel();
        
        // Set Observe

        if (gameTilesModel.TilesProperty.HasValue)
            gameTilesModel.TilesProperty.AsObservable().Subscribe(newTilesMap =>
            {
                gameTilesView.SetLayout(newTilesMap);
            }).AddTo(gameTilesView);
    }
}
