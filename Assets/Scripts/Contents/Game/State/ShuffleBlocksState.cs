using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffleBlocksState : IState
{
    public void Begin()
    {
        Debug.Log("셔플 고고");
        Managers.Game.GamePresenter.RequestShuffleBlocks();
    }

    public void Process()
    {
    }

    public void End()
    {
    }
}
