using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Logic;
using UnityEngine;
using Util;

public class TileView : MonoBehaviour, BaseMVP.IView
{
    [SerializeField] private SpriteRenderer tileImage;
    
    public void SetLayout<T>(T data)
    {
        if(!(data is Tile tile)) return;
        
        var defaultTileScale = Managers.Data.ConstantsTableData.TileDefaultScale;
        transform.localScale = new Vector3(defaultTileScale, defaultTileScale, 0);
        transform.localPosition = GameUtil.ConvertVec3ToHexVet3(new Vector3(tile.TilePos.x,tile.TilePos.y,0));

        // 이미지 스프라이트가 없으므로 색깔만 변경
        tileImage.color = tile.TileColor switch
        {
            TileColor.Gray => new Color(42 / 255f, 50 / 255f, 84 / 255f),
        };

        switch (tile.Type)
        {
            case TileType.Empty:
                gameObject.SetActive(false);
                break;
            case TileType.Normal:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public GameObject GetInstance() => gameObject;
}
