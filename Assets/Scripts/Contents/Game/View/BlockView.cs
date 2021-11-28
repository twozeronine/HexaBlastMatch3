using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Logic;
using UnityEngine;
using Util;

public class BlockView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer blockImage;

    public void SetLayout<T>(T data)
    {
        if(!(data is Block block)) return;

        var defaultBlockScale = Managers.Data.ConstantsTableData.BlockDefaultScale;
        transform.localPosition = GameUtil.ConvertVec3ToHexVet3(new Vector3(block.BlockPos.x, block.BlockPos.y, 0));
        transform.localScale = new Vector3(defaultBlockScale, defaultBlockScale, 1);
        
        // 임시
        blockImage.color = block.Color switch
        {
            BlockColor.Red => Color.red,
            BlockColor.Yellow => Color.yellow,
            BlockColor.Green => Color.green,
            BlockColor.Blue => Color.blue,
            BlockColor.Purple => Color.magenta,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
