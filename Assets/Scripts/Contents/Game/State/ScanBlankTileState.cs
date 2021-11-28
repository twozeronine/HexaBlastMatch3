using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanBlankTileState : IState
{
    public void Begin()
    {
        Debug.Log("빈타일 매꾸기 시작");
    }

    public void Process()
    {
        Managers.Game.GamePresenter.RequestScanBlankTile();
    }

    public void End()
    {
    }
}
