using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using UnityEngine.EventSystems;
using Logic;

public enum EInputCommandType
{
    Tap,
    Zoom,
    Drag,
    BackKey,
    TapOff
}

public struct InputData
{
    public EInputCommandType InputType;
    public Vector2 CurrentPos;
    public Vector2 DeltaPos;
    public Ray Ray;
    public float Zoom;
}

public delegate void InputEvent(InputData inputData);

public class InputManager
{
    private LeanTouch leanTouch;
    private readonly Dictionary<EInputCommandType, InputEvent> handlerMap = new Dictionary<EInputCommandType, InputEvent>();
    private int pointerID;

    public void Init()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        pointerID = -1; //OnClick
#else
		pointerID = 0;  //OnTouch
#endif
        GameObject.Find("@Managers").AddComponent<LeanTouch>();
        
        LeanTouch.OnFingerTap += OnTap;
        LeanTouch.OnFingerUp += OffTap;
        
    }

    public void Clear()
    {
        LeanTouch.OnFingerTap -= OnTap;
        LeanTouch.OnFingerUp -= OffTap;
    }


    public void OnUpdate()
    {
        if (Application.platform == RuntimePlatform.Android
            || Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                var inputData = new InputData()
                {
                    InputType = EInputCommandType.BackKey
                };

                Dispatch(inputData);
            }
        }

        if (EventSystem.current == null)
            return;

        if (EventSystem.current.IsPointerOverGameObject(pointerID))
        {
            return;
        }

        var fingers = LeanTouch.GetFingers(false, false);

        if (fingers.Count == 1)
        {
            var ray = fingers[0].GetRay();
            if (Camera.main is null) return;

            var inputData = new InputData()
            {
                InputType = EInputCommandType.Drag,
                DeltaPos = LogicUtil.ScreenPosToWorldPos(fingers[0].LastScreenPosition) -
                           LogicUtil.ScreenPosToWorldPos(fingers[0].ScreenPosition),
                CurrentPos = ray.origin,
                Ray = ray
            };

            Dispatch(inputData);

            
            return;
        }

        var pinchRatio = LeanGesture.GetPinchRatio(fingers, 0.1f);
        var pinchScale = LeanGesture.GetPinchScale(fingers, 0.1f);

        if (!Mathf.Approximately(pinchRatio, 1.0f))
        {
            var inputData = new InputData()
            {
                InputType = EInputCommandType.Zoom,
                Zoom = pinchRatio
            };

            Dispatch(inputData);
        }
    }

    public void On(EInputCommandType commandType, InputEvent inputEvent)
    {
        if (!handlerMap.ContainsKey(commandType))
        {
            handlerMap.Add(commandType, default(InputEvent));
        }

        handlerMap[commandType] += inputEvent;
    }

    public void Off(EInputCommandType commandType, InputEvent inputEvent)
    {
        if (!handlerMap.ContainsKey(commandType))
        {
            return;
        }

        handlerMap[commandType] -= inputEvent;
    }

    private void OnTap(LeanFinger finger)
    {
        if (finger.StartedOverGui)
        {
            return;
        }

        var ray = finger.GetRay();
        var inputData = new InputData()
        {
            InputType = EInputCommandType.Tap,
            CurrentPos = ray.origin,
            Ray = ray
        };

        Dispatch(inputData);
    }

    private void OffTap(LeanFinger finger)
    {
        if (finger.StartedOverGui)
        {
            return;
        }

        var ray = finger.GetRay();
        var inputData = new InputData()
        {
            InputType = EInputCommandType.TapOff,
            CurrentPos = ray.origin,
            Ray = ray
        };

        Dispatch(inputData);
    }

    private void Dispatch(InputData data)
    {
        if (!handlerMap.TryGetValue(data.InputType, out var inputEvent))
        {
            return;
        }

        inputEvent?.Invoke(data);
    }
}