using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitUserTapOff : IState
{
    public void Begin()
    {
        Debug.Log("손가락을 떼시오.");
    }

    public void Process()
    {
    }

    public void End()
    {
    }
}
