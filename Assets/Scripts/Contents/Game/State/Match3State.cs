using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3State : IState
{
    public void Begin()
    {
        Debug.Log("블록제거 연산중");
    }

    public void Process()
    {
    }

    public void End()
    {
    }
}
