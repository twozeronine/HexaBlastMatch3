using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class GameBlocksView : MonoBehaviour , BaseMVP.IView
{
    private GameBlocksPresenter gameBlocksPresenter;
    private List<GameObject> blocksList = new List<GameObject>();

    private void Start()
    {
        gameBlocksPresenter = new GameBlocksPresenter(this);
        gameBlocksPresenter.CreateModel();
        gameBlocksPresenter.LoadData();
    }
    
    public void SetLayout<T>(T data)
    {
        if(!(data is Blocks blocks)) return;

        foreach (var block in blocks.BlocksMap.Values)
        {
            var blockObj = Managers.Resource.Instantiate("Game/Block", transform);
            blockObj.GetOrAddComponent<Poolable>();

            var blockView = blockObj.GetOrAddComponent<BlockView>();
            blockView.SetLayout(block);
        }
    }

    public GameObject GetInstance()
    {
        throw new System.NotImplementedException();
    }
}
