using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Logic;
using UnityEngine;

public class GameTilesView : MonoBehaviour ,BaseMVP.IView
{
    private GameTilesPresenter gameTilesPresenter;
    private List<GameObject> tileList = new List<GameObject>();

    private void Start()
    {
        gameTilesPresenter = new GameTilesPresenter(this);
        gameTilesPresenter.CreateModel();
        gameTilesPresenter.LoadData();
    }

    public void SetLayout<T>(T data)
    {
        if(!(data is Tiles tiles)) return;

        foreach (var tile in tileList)
        {
            Managers.Resource.Destroy(tile);
        }
        
        tileList.Clear();

        foreach (var tile in tiles.TilesMap.Values)
        {
           var tileObj = Managers.Resource.Instantiate("Game/Tile", transform);
            tileObj.GetOrAddComponent<Poolable>();
            var tileView = tileObj.GetOrAddComponent<TileView>();
            tile.TileColor = TileColor.Gray;
            tileView.SetLayout(tile);
            tileList.Add(tileObj);
        }
    }

    public GameObject GetInstance() => gameObject;
}
