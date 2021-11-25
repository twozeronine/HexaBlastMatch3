using System.Collections.Generic;
using Logic;
using UnityEngine;

public class UIManager
{
    // Popup끼리의 order
    private int order = 10;

    private readonly Stack<UIPopup> popupStack = new Stack<UIPopup>();
    private UIScene sceneUI;

    public GameObject Root
    {
        get
        {
            var root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }

    // 외부에서 Pop창이 열어질때 Canvas의 우선순위를 정해줌.
    public void SetCanvas(GameObject go, bool sort = true)
    {
        var canvas = LogicUtil.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        // canvas 안에 canvas가 있을때 부모가 어떤 값을 가지던 자신의 sortingorder값을 가짐
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = order;
            order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UIBase
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        var go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}");

        if (parent != null)
            go.transform.SetParent(parent);

        var canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return LogicUtil.GetOrAddComponent<T>(go);
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UIBase
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        var go = Managers.Resource.Instantiate($"UI/SubItem/{name}");

        if (parent != null)
            go.transform.SetParent(parent);

        return LogicUtil.GetOrAddComponent<T>(go);
    }

    public T ShowSceneUI<T>(string name = null) where T : UIScene
    {
        //string을 안넣었을경우 Type으로 받아온다.
        //Type과 string을 일치 시켰기때문에 가능함.
        // ex ) UI_Button 프리팹에 붙어있는 스크립트는 UI_Button 
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;


        var go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        // 혹시라도 Prefab에 컴포넌트를 안붙여놨을경우
        var thisSceneUI = LogicUtil.GetOrAddComponent<T>(go);
        this.sceneUI = thisSceneUI;

        go.transform.SetParent(Root.transform);

        return thisSceneUI;
    }

    public T ShowPopupUI<T>(string name = null) where T : UIPopup
    {
        //string을 안넣었을경우 Type으로 받아온다.
        //Type과 string을 일치 시켰기때문에 가능함.
        // ex ) UI_Button 프리팹에 붙어있는 스크립트는 UI_Button 
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        var go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        // 혹시라도 Prefab에 컴포넌트를 안붙여놨을경우
        var popup = LogicUtil.GetOrAddComponent<T>(go);
        popupStack.Push(popup);

        go.transform.SetParent(Root.transform);

        return popup;
    }


    // 제일 마지막으로 뜬 팝업창을 꺼줌.
    // 스택에 쌓인 순서대로 팝업이 켜졌을테니 순서대로 종료됌.
    public void ClosePopupUI()
    {
        if (popupStack.Count == 0) return;

        var popup = popupStack.Pop();
        // popup 컴포넌트를 가진 게임오브젝트를 삭제.
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
    }

    // 혹시라도 다른 스크립트에서 팝업을 종료했을시
    // 해당 팝업이 아닌 다른 팝업을 종료할 수 도있기 때문에 안전하게 제거하기 위한 메소드
    public void ClosePopupUI(UIPopup popup)
    {
        if (popupStack.Count == 0) return;

        if (popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        ClosePopupUI();
    }

    public void CloseAllPopupUI()
    {
        while (popupStack.Count > 0)
            ClosePopupUI();
    }


    public void Clear()
    {
        CloseAllPopupUI();
        sceneUI = null;
    }
}