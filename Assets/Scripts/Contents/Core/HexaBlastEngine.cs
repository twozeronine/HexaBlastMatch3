using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Logic;
using UnityEngine;

public static class HexaBlastEngine
{
    public static Blocks InitBlock(Tiles tiles)
    {
        var canMakeBlockPos = from canMakeBlockTile in tiles.TilesMap.Values
            where canMakeBlockTile.ChildBlock.BlockType == BlockType.Empty && canMakeBlockTile.IsValid
            select canMakeBlockTile.TilePos;

        var resultBlocksMap = canMakeBlockPos.ToDictionary(vector2Int => vector2Int, vector2Int => new Block()
        {
            BlockType = BlockType.Normal,
            Color = FuncUtil.GetRandomEnumValue<BlockColor>(),
            IsValid = true,
            ParentTile = tiles.GetTile(vector2Int),
            BlockPos = tiles.GetTile(vector2Int).TilePos,
        });

        return new Blocks()
        {
            BlocksMap = resultBlocksMap,
        };
    }

    public static Blocks ShuffleInitBlock(Blocks blocks)
    {
        return default;
    }
}
