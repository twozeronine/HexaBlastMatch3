using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using Logic;
using UnityEngine;
using UnityEngine.UI;

public class GameTopUIView : UIScene, BaseMVP.IView
{
    private enum Images
    {
        MissionImage,
        FillGage,
    }

    private enum Texts
    {
        MissionCountText,
        MoveCountText,
        ScoreText,
    }

    private GameTopUIPresenter gameTopUIPresenter;

    [NonSerialized] public Text MissionCountText;
    [NonSerialized] public Text MoveCountText;
    [NonSerialized] public Text ScoreText;
    
    [NonSerialized] public Image MissionImage;
    [NonSerialized] public Image FillGage;

    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));
        

        MissionCountText = GetText((int)Texts.MissionCountText);
        MoveCountText = GetText((int)Texts.MoveCountText);
        ScoreText = GetText((int)Texts.ScoreText);

        MissionImage = GetImage((int)Images.MissionImage);
        FillGage = GetImage((int)Images.FillGage);

        gameTopUIPresenter = new GameTopUIPresenter(this);
        gameTopUIPresenter.CreateModel();
        gameTopUIPresenter.LoadData();
    }
    
    public void SetLayout<T>(T data)
    {
        if(!(data is StageTableData stageData)) return;

        Debug.Log(stageData.MissionCount);
        
        MissionCountText.text = $"{stageData.MissionCount}";
        MoveCountText.text = $"{stageData.MoveCount}";
        MissionImage.color =(StageMissionType)Enum.Parse(typeof(StageMissionType), stageData.MissionType) switch
        {
            StageMissionType.Top => Color.cyan,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void SetScore(int score)
    {
        ScoreText.DOText(score.ToString(), 0.5f, true);
    }
}
