using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logic;

public class SceneManagerEx
{
    public BaseScene CurrentScene { get => GameObject.FindObjectOfType<BaseScene>(); }

    public void LoadScene(Logic.Scene type)
    {
        SceneManager.LoadScene(FuncUtil.EnumToString(type));
    }
    public void Clear()
    {
        CurrentScene.Clear();
    }
}