using System;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public abstract class UIBase : MonoBehaviour
{
    protected readonly Dictionary<Type, UnityEngine.Object[]> Objects = new Dictionary<Type, UnityEngine.Object[]>();

    public abstract void Init();

    private void Start()
    {
        Init();
    }

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        // Enum 타입에서 이름을 string으로 얻어옴
        var names = Enum.GetNames(type);
        // Enum에서 선언한 갯수만큼 object배열을 만듬
        var bindObjects = new UnityEngine.Object[names.Length];
        // 해당 배열을 딕셔너리에 넣음 나중에 Type을 통해서 배열 접근 가능.
        this.Objects.Add(typeof(T), bindObjects);

        for (var i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                bindObjects[i] = LogicUtil.FindChild(gameObject, names[i], true);
            else
                bindObjects[i] = LogicUtil.FindChild<T>(gameObject, names[i], true);

            if (bindObjects[i] == null)
                Debug.Log($"Failed to bind({names[i]})");
        }
    }

    private T Get<T>(int idx) where T : UnityEngine.Object
    {
        //딕셔너리에서 값을 꺼내옴. 
        if (this.Objects.TryGetValue(typeof(T), out var bindObjects) == false)
            return null;

        return bindObjects[idx] as T;
    }

    protected GameObject GetObject(int idx)
    {
        return Get<GameObject>(idx);
    }

    protected Text GetText(int idx)
    {
        return Get<Text>(idx);
    }

    protected Button GetButton(int idx)
    {
        return Get<Button>(idx);
    }

    protected Image GetImage(int idx)
    {
        return Get<Image>(idx);
    }

    public static void BindEvent(GameObject go, Action<PointerEventData> action,
        UIEvent type = UIEvent.Click)
    {
        var evt = LogicUtil.GetOrAddComponent<UIEventHandler>(go);

        switch (type)
        {
            case UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    public T UIBind<T>(int uiIndex) where T : UnityEngine.Object => Get<T>(uiIndex);
    public GameObject GetInstance() => gameObject;
}