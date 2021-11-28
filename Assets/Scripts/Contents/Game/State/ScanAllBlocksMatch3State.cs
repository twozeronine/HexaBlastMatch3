using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanAllBlocksMatch3State : IState
{
    public void Begin()
    {
        Debug.Log("매칭 후 매치3 검사하기 시작 !!");
    }

    public void Process()
    {
        Managers.Game.GamePresenter.RequestScanAllBlocksMatch3();
    }

    public void End()
    {
    }
}
