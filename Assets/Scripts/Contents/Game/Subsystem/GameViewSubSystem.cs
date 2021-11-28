using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using UniRx;
using UnityEngine;
using Util;

public class GameViewSubSystem
{
    public readonly Subject<Vector2Int> DragBlockSubject = new Subject<Vector2Int>();
    public readonly Subject<Unit> TapOffSubject = new Subject<Unit>();

    public bool IsTapOff = true;
    public void Init()
    {
        Managers.Input.On(EInputCommandType.Drag,DispatchInputEvent);
        Managers.Input.On(EInputCommandType.TapOff,DispatchInputEvent);
    }
    
    private void DispatchInputEvent(InputData inputData)
    {
        switch (inputData.InputType)
        {
            case EInputCommandType.Tap:
                break;
            case EInputCommandType.Drag:
                OnDragInputEvent(inputData);
                break;
            case EInputCommandType.TapOff:
                OffTapInputEvent(inputData);
                break;
            case EInputCommandType.Zoom:
                break;
            case EInputCommandType.BackKey:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnDragInputEvent(InputData inputData)
    {
        
        if(Managers.Game.CurrentGameState() != EGameState.WaitUserInput || !IsTapOff) return;
        
        // Ray로 검출
        var hit = Physics2D.Raycast(inputData.Ray.origin,inputData.Ray.direction);
        if (!hit) return;

        var localPosition = hit.transform.localPosition;
        var blockVec3 = GameUtil.ConvertHexVet3ToVec3( new Vector3(localPosition.x, localPosition.y,0));
        DragBlockSubject.OnNext(new Vector2Int((int)blockVec3.x,(int)blockVec3.y));
    }

    private  void OffTapInputEvent(InputData inputData)
    {
        TapOffSubject.OnNext(Unit.Default);
        IsTapOff = true;
    }
}
