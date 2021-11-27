using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    // Model
    public GameTilesModel GameTilesModel { get; set; } = new GameTilesModel();
    
    // Presenter
    private GamePresenter gamePresenter { get; set; } = new GamePresenter();

    // SubSystem

    public void Init()
    {
        gamePresenter.Init();   
    }
}
public class GamePresenter
{
    // View
    private GameTilesView gameTilesView;

    public void Init()
    {
        var gameTileViewGo = Managers.Resource.Instantiate("Game/GameBoardLayout");
        gameTilesView = gameTileViewGo.GetComponentInChildren<GameTilesView>();
    }
}