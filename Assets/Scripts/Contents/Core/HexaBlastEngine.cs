using System;
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

        var resultBlocksMap = canMakeBlockPos.ToDictionary(vector2Int => vector2Int, vector2Int =>
        {
            var newBlock = new Block()
            {
                BlockType = BlockType.Normal,
                Color = FuncUtil.GetRandomEnumValue<BlockColor>(),
                IsValid = true,
                ParentTile = tiles.GetTile(vector2Int),
                BlockPos = tiles.GetTile(vector2Int).TilePos,
            };

            tiles.GetTile(vector2Int).ChildBlock = newBlock;

            return newBlock;
        });

        return new Blocks()
        {
            BlocksMap = resultBlocksMap,
        };
    }

    // 이미 매치3가 되어있는지 블럭을 빠르게 검사하는 로직
    public static List<MatchedBlock> ScanInitMatched3Blocks(Blocks blocks)
    {
        return blocks.BlocksMap.Values.Select(block => new MatchedBlock
        {
            BlockPos = block.BlockPos,
            Color = block.Color,
            IsMatched3TopRight = CheckMatch3Pattern(block, blocks, Direction.TopRight),
            IsMatched3Top = CheckMatch3Pattern(block, blocks, Direction.Top),
            IsMatched3BottomRight = CheckMatch3Pattern(block, blocks, Direction.BottomRight)
        }).Where(matchedBlock => matchedBlock.IsMatched3Top || matchedBlock.IsMatched3BottomRight ||
                                 matchedBlock.IsMatched3TopRight).ToList();
    }

    public static void ShuffledMatched3Blocks(ref Blocks preBlocks,List<MatchedBlock> mustShuffledBlocks)
    {
        foreach (var mustShuffledBlock in mustShuffledBlocks)
        {
            var nextBlock = preBlocks.GetBlock(mustShuffledBlock.BlockPos);
            nextBlock.Color = FuncUtil.GetRandomEnumValue<BlockColor>(mustShuffledBlock.Color);
            preBlocks.BlocksMap[nextBlock.BlockPos].Color = nextBlock.Color;
        }
    }
    
    // 매치 3가 되었나 검사 하는 로직
    public static bool CheckMatch3Pattern(Block block, Blocks blocksMap, Direction dir, int depth = 0,
        int matchCountNumber = 0)
    {
        var nextBlock = HexaBlastEngineUtil.GetBlock(blocksMap, dir, block.BlockPos);

        // 블럭 검사 조건문
        // 장애물 추가시 장애물 관련 조건 추가한다.
        if (nextBlock is { IsValid: true } && nextBlock.Color.Equals(block.Color) && depth < 3 + matchCountNumber)
        {
            return CheckMatch3Pattern(nextBlock, blocksMap, dir, ++depth, matchCountNumber);
        }

        return depth >= 2 + matchCountNumber;
    }
}

public static class HexaBlastEngineUtil
{
    public static Block GetBlock(Blocks blocks, Direction direction, Vector2Int targetBlockPos)
        => direction switch
        {
            Direction.None => blocks.GetBlock(new Vector2Int(targetBlockPos.x, targetBlockPos.y)),
            Direction.Top => blocks.GetBlock(new Vector2Int(targetBlockPos.x, targetBlockPos.y + 2)),
            Direction.Bottom => blocks.GetBlock(new Vector2Int(targetBlockPos.x, targetBlockPos.y - 2)),
            Direction.TopRight => blocks.GetBlock(new Vector2Int(targetBlockPos.x + 1, targetBlockPos.y + 1)),
            Direction.TopLeft => blocks.GetBlock(new Vector2Int(targetBlockPos.x - 1, targetBlockPos.y + 1)),
            Direction.BottomLeft => blocks.GetBlock(new Vector2Int(targetBlockPos.x - 1, targetBlockPos.y -1)),
            Direction.BottomRight => blocks.GetBlock(new Vector2Int(targetBlockPos.x + 1, targetBlockPos.y - 1)),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
}
