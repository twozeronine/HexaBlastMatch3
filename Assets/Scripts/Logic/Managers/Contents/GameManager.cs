using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    // Model
    public GameTilesModel GameTilesModel { get; set; } = new GameTilesModel();
    public GameBlocksModel GameBlocksModel { get; set; } = new GameBlocksModel();
    
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
    private GameBlocksView gameBlocksView;
    
    public void Init()
    {
        var gameBoardLayout = Managers.Resource.Instantiate("Game/GameBoardLayout");
        gameTilesView = gameBoardLayout.GetComponentInChildren<GameTilesView>();
        gameBlocksView = gameBoardLayout.GetComponentInChildren<GameBlocksView>();
    }
}