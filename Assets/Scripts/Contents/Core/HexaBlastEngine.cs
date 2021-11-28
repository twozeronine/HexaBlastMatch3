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

    // 블럭을 셔플하는 로직
    public static void ShuffledMatched3Blocks(ref Blocks preBlocks,List<MatchedBlock> mustShuffledBlocks)
    {
        foreach (var mustShuffledBlock in mustShuffledBlocks)
        {
            var nextBlock = preBlocks.GetBlock(mustShuffledBlock.BlockPos);
            nextBlock.Color = FuncUtil.GetRandomEnumValue<BlockColor>(mustShuffledBlock.Color);
            preBlocks.BlocksMap[nextBlock.BlockPos].Color = nextBlock.Color;
        }
    }
    
    // 제거 할 수 있는 블럭이 있는지 빠르게 체크하는 로직
    // 다음에 힌트 검사 로직은 특수블럭, 5,4개만 검사함.
    public static bool TryCheckCanRemoveBlocks(Blocks preBlocks, out Block hintBlock)
    {
        var result = (from preBlock in preBlocks.BlocksMap.Values
            where
                TryCheckMatch3(preBlock, preBlocks, Direction.Top) ||
                TryCheckMatch3(preBlock, preBlocks, Direction.Bottom) ||
                TryCheckMatch3(preBlock, preBlocks, Direction.TopRight) ||
                TryCheckMatch3(preBlock, preBlocks, Direction.BottomRight) ||
                TryCheckMatch3(preBlock, preBlocks, Direction.TopLeft) ||
                TryCheckMatch3(preBlock, preBlocks, Direction.BottomLeft) 
            select preBlock).ToList();

        hintBlock = result.FirstOrDefault();
        return result.Any();
    }
    
    public static bool TryCheckMatch3(Block block, Blocks blocks, Direction scanDir)
    {
        var nextBlock = HexaBlastEngineUtil.GetBlock(blocks, scanDir, block.BlockPos);
        if (nextBlock == null) return false;

        // 가상으로 블록을 서로 교환한 상태로 만듬.
        var nextBlocks = new Blocks()
        {
            BlocksMap = new Dictionary<Vector2Int, Block>(blocks.BlocksMap)
            {
                [nextBlock.BlockPos] = new Block()
                {
                    BlockPos = block.BlockPos,
                    BlockType = block.BlockType,
                    Color = block.Color,
                    ParentTile = block.ParentTile,
                    IsValid = block.IsValid,
                },
                [block.BlockPos] = new Block()
                {
                    BlockPos = nextBlock.BlockPos,
                    BlockType = block.BlockType,
                    Color = nextBlock.Color,
                    ParentTile = nextBlock.ParentTile,
                    IsValid = nextBlock.IsValid,
                }
            }
        };

        nextBlocks.BlocksMap[nextBlock.BlockPos].BlockPos = nextBlock.BlockPos;

        // 가상으로 교환한 방향의 반대 방향으로의 검색은 하지않음
        var exceptDirection = scanDir switch
        {
            Direction.Top => Direction.Bottom,
            Direction.Bottom => Direction.Top,
            Direction.TopLeft => Direction.BottomRight,
            Direction.TopRight => Direction.BottomLeft,
            Direction.BottomLeft => Direction.TopRight,
            Direction.BottomRight => Direction.TopLeft,
            _ => throw new ArgumentOutOfRangeException(nameof(scanDir), scanDir, null)
        };

        var result = (from direction in Enum.GetValues(typeof(Direction))
                .Cast<Direction>()
            where !direction.Equals(exceptDirection)
            select CheckMatch3Pattern(nextBlocks.BlocksMap[nextBlock.BlockPos], nextBlocks, direction) ||
                   CheckMatch3MiddleY(nextBlocks.BlocksMap[nextBlock.BlockPos], nextBlocks) ||
                   CheckMatch3MiddleDiagonalLowerLeft(nextBlocks.BlocksMap[nextBlock.BlockPos], nextBlocks) ||
                   CheckMatch3MiddleDiagonalLowerRight(nextBlocks.BlocksMap[nextBlock.BlockPos], nextBlocks)
                   ).ToList();

        return result.Any(isTrue => isTrue);
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
    
    /*  Pattern 1 
     *  1 
     *  1 <-- 이쪽 블럭이 움직여져서 매치3 가 된 경우 탐지
     *  1 
     *  하는 로직
     */
    public static bool CheckMatch3MiddleY(Block block, Blocks blocksMap)
    {
        var nextTopBlock = HexaBlastEngineUtil.GetBlock(blocksMap, Direction.Top, block.BlockPos);
        var nextBottomBlock = HexaBlastEngineUtil.GetBlock(blocksMap, Direction.Bottom, block.BlockPos);

        return nextTopBlock is { IsValid: true } && nextTopBlock.Color.Equals(block.Color) &&
               nextBottomBlock is { IsValid: true } && nextBottomBlock.Color.Equals(block.Color);
    }
    
    /*  Pattern 2 
     *  1 
     *    1 <-- 이쪽 블럭이 움직여져서 매치3 가 된 경우 탐지
     *      1 
     *  하는 로직
     */
    public static bool CheckMatch3MiddleDiagonalLowerRight(Block block, Blocks blocksMap)
    {
        var nextTopLeftBlock = HexaBlastEngineUtil.GetBlock(blocksMap, Direction.TopLeft, block.BlockPos);
        var nextBottomRightBlock = HexaBlastEngineUtil.GetBlock(blocksMap, Direction.BottomRight, block.BlockPos);

        return nextTopLeftBlock is { IsValid: true } && nextTopLeftBlock.Color.Equals(block.Color) &&
               nextBottomRightBlock is { IsValid: true } && nextBottomRightBlock.Color.Equals(block.Color);
    }
    
    /*  Pattern 3 
     *      1
     *    1 <-- 이쪽 블럭이 움직여져서 매치3 가 된 경우 탐지
     *  1 
     *  하는 로직
     */
    public static bool CheckMatch3MiddleDiagonalLowerLeft(Block block, Blocks blocksMap)
    {
        var nextTopRightBlock = HexaBlastEngineUtil.GetBlock(blocksMap, Direction.TopRight, block.BlockPos);
        var nextBottomLeftBlock = HexaBlastEngineUtil.GetBlock(blocksMap, Direction.BottomLeft, block.BlockPos);

        return nextTopRightBlock is { IsValid: true } && nextTopRightBlock.Color.Equals(block.Color) &&
               nextBottomLeftBlock is { IsValid: true } && nextBottomLeftBlock.Color.Equals(block.Color);
    }
    
    // Match 4 감지하는 로직
    public static bool CheckMatch4Pattern(Block block, Blocks blocksMap, Direction dir, int depth = 0) =>
        CheckMatch3Pattern(block, blocksMap, dir, depth, 1);
    
    public static bool CheckMatch4MiddleY(Block block, Blocks blocksMap, bool isTop)
    {
        var nextTopBlock = HexaBlastEngineUtil.GetBlock(blocksMap, Direction.Top, block.BlockPos);
        var nextBottomBlock = HexaBlastEngineUtil.GetBlock(blocksMap, Direction.Bottom, block.BlockPos);

        if (nextTopBlock == null || nextBottomBlock == null) return false;

        var nextExtraBlock = HexaBlastEngineUtil.GetBlock(blocksMap, isTop ? Direction.Top : Direction.Bottom,
            isTop ? nextTopBlock.BlockPos : nextBottomBlock.BlockPos);

        return nextTopBlock is { IsValid: true } && nextTopBlock.Color.Equals(block.Color) &&
               nextBottomBlock is { IsValid: true } && nextBottomBlock.Color.Equals(block.Color) &&
               nextExtraBlock is { IsValid: true } && nextExtraBlock.Color.Equals(block.Color);
    }
    
    /*
     * 1
     *   1  <-- 이 블럭을 움직여 매치 4가 된 경우. isUp = true
     *     1   <-- 이 블럭을 움직여 매치 4가 된 경우. isUp = false
     *       1 
     */
    public static bool CheckMatch4MiddleDiagonalLowerRight(Block block, Blocks blocksMap, bool isUp)
    {
        var nextTopLeftBlock = HexaBlastEngineUtil.GetBlock(blocksMap, Direction.TopLeft, block.BlockPos);
        var nextBottomRightBlock = HexaBlastEngineUtil.GetBlock(blocksMap, Direction.BottomRight, block.BlockPos);

        if (nextTopLeftBlock == null || nextBottomRightBlock == null) return false;

        var nextExtraBlock = HexaBlastEngineUtil.GetBlock(blocksMap, isUp ? Direction.BottomRight : Direction.TopLeft,
            isUp ? nextBottomRightBlock.BlockPos : nextTopLeftBlock.BlockPos);


        return nextTopLeftBlock is { IsValid: true } && nextTopLeftBlock.Color.Equals(block.Color) &&
               nextBottomRightBlock is { IsValid: true } && nextBottomRightBlock.Color.Equals(block.Color) &&
               nextExtraBlock is { IsValid: true } && nextExtraBlock.Color.Equals(block.Color);
    }
    
    /*
     *          1
     *       1 <-- 이 블럭을 움직여 매치 4가 된 경우. isUp = true
     *     1   <-- 이 블럭을 움직여 매치 4가 된 경우. isUp = false
     *   1 
     */
    public static bool CheckMatch4MiddleDiagonalLowerLeft(Block block, Blocks blocksMap, bool isUp)
    {
        var nextTopRightBlock = HexaBlastEngineUtil.GetBlock(blocksMap, Direction.TopRight, block.BlockPos);
        var nextBottomLeftBlock = HexaBlastEngineUtil.GetBlock(blocksMap, Direction.BottomLeft, block.BlockPos);

        if (nextTopRightBlock == null || nextBottomLeftBlock == null) return false;

        var nextExtraBlock = HexaBlastEngineUtil.GetBlock(blocksMap, isUp ? Direction.BottomLeft : Direction.TopRight ,
            isUp ? nextBottomLeftBlock.BlockPos :  nextTopRightBlock.BlockPos);

        return nextTopRightBlock is { IsValid: true } && nextTopRightBlock.Color.Equals(block.Color) &&
               nextBottomLeftBlock is { IsValid: true } && nextBottomLeftBlock.Color.Equals(block.Color) &&
               nextExtraBlock is { IsValid: true } && nextExtraBlock.Color.Equals(block.Color);
    }

    
    public static MatchedBlock ScanMatch3(Block targetBlock, Blocks blocks,Vector2Int prevPos = default)
    {
        if (prevPos == default) prevPos = targetBlock.BlockPos;

        // Match 4

        var isMatched4Top = CheckMatch4Pattern(targetBlock, blocks, Direction.Top); 
        var isMatched4TopRight = CheckMatch4Pattern(targetBlock, blocks, Direction.TopRight);
        var isMatched4TopLeft = CheckMatch4Pattern(targetBlock, blocks, Direction.TopLeft);
        var isMatched4Bottom =  CheckMatch4Pattern(targetBlock, blocks, Direction.Bottom);
        var isMatched4BottomLeft =CheckMatch4Pattern(targetBlock, blocks, Direction.BottomLeft);
        var isMatched4BottomRight =CheckMatch4Pattern(targetBlock, blocks, Direction.BottomRight);
        var isMatched4MiddleDiagonalLowerLeftUp = CheckMatch4MiddleDiagonalLowerLeft(targetBlock, blocks, true);
        var isMatched4MiddleDiagonalLowerLeftDown = CheckMatch4MiddleDiagonalLowerLeft(targetBlock, blocks, false);
        var isMatched4MiddleDiagonalLowerRightUp = CheckMatch4MiddleDiagonalLowerRight(targetBlock, blocks, true);
        var isMatched4MiddleDiagonalLowerRightDown = CheckMatch4MiddleDiagonalLowerRight(targetBlock, blocks, false);
        var isMatched4MiddleUpperY = CheckMatch4MiddleY(targetBlock, blocks, true);
        var isMatched4MiddleLowerY = CheckMatch4MiddleY(targetBlock, blocks, true);
        
        // Match 3
        var isMatched3Top = !isMatched4Top && CheckMatch3Pattern(targetBlock, blocks, Direction.Top);
        var isMatched3TopRight = !(isMatched4TopRight || isMatched4MiddleDiagonalLowerLeftDown )&& CheckMatch3Pattern(targetBlock, blocks, Direction.TopRight);
        var isMatched3TopLeft = !(isMatched4TopLeft || isMatched4MiddleDiagonalLowerRightDown )&& CheckMatch3Pattern(targetBlock, blocks, Direction.TopLeft);
        var isMatched3Bottom = !isMatched4Bottom && CheckMatch3Pattern(targetBlock, blocks, Direction.Bottom);
        var isMatched3BottomLeft = !(isMatched4BottomLeft || isMatched4MiddleDiagonalLowerLeftUp) && CheckMatch3Pattern(targetBlock, blocks, Direction.BottomLeft);
        var isMatched3BottomRight = !(isMatched4BottomRight || isMatched4MiddleDiagonalLowerRightUp ) && CheckMatch3Pattern(targetBlock, blocks, Direction.BottomRight);
        var isMatched3MiddleDiagonalLeftDown = !(isMatched4MiddleDiagonalLowerLeftUp || isMatched4MiddleDiagonalLowerLeftDown) && CheckMatch3MiddleDiagonalLowerLeft(targetBlock, blocks);
        var isMatched3MiddleXDiagonalRightDown = !(isMatched4MiddleDiagonalLowerRightUp || isMatched4MiddleDiagonalLowerRightDown ) && CheckMatch3MiddleDiagonalLowerRight(targetBlock, blocks);
        var isMatched3MiddleY = !(isMatched4MiddleUpperY ||isMatched4MiddleLowerY ) && CheckMatch3MiddleY(targetBlock, blocks);

        return new MatchedBlock()
        {
            PrevBlockPos = prevPos,
            BlockPos = targetBlock.BlockPos,
            Color = targetBlock.Color,
            BlockType = targetBlock.BlockType,
            IsMatched3Top = isMatched3Top,
            IsMatched3TopLeft = isMatched3TopLeft,
            IsMatched3TopRight = isMatched3TopRight,
            IsMatched3Bottom = isMatched3Bottom,
            IsMatched3BottomLeft = isMatched3BottomLeft, 
            IsMatched3BottomRight = isMatched3BottomRight,
            IsMatched3MiddleDiagonalLowerLeft = isMatched3MiddleDiagonalLeftDown,
            IsMatched3MiddleDiagonalLowerRight = isMatched3MiddleXDiagonalRightDown,
            IsMatched3MiddleY = isMatched3MiddleY,
            IsMatched4Top           = isMatched4Top,
            IsMatched4TopLeft       = isMatched4TopLeft,
            IsMatched4TopRight      = isMatched4TopRight,
            IsMatched4Bottom        = isMatched4Bottom,
            IsMatched4BottomRight   = isMatched4BottomRight,
            IsMatched4BottomLeft    = isMatched4BottomLeft,
            IsMatched4MiddleUpperY  = isMatched4MiddleUpperY,
            IsMatched4MiddleLowerY  = isMatched4MiddleLowerY,
            IsMatched4MiddleDiagonalLowerLeftUp    = isMatched4MiddleDiagonalLowerLeftUp,
            IsMatched4MiddleDiagonalLowerLeftDown  = isMatched4MiddleDiagonalLowerLeftDown,
            IsMatched4MiddleDiagonalLowerRightUp   = isMatched4MiddleDiagonalLowerRightUp,
            IsMatched4MiddleDiagonalLowerRightDown = isMatched4MiddleDiagonalLowerRightDown,
        };
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
    
        public static List<MovableBlockView> GetMatchedBlocks(Blocks blocks, MatchedBlock matchedBlock,bool isSwapBlock = false,Direction swapDirection = Direction.None)
    {
        var removeBlocks = new List<MovableBlockView>();
        
/*
         * Match 4 검사 
         */

        // Top

        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                  !matchedBlock.IsMatched3Top &&
                  matchedBlock.IsMatched4Top
                  && matchedBlock.BlockPos.x == block.BlockPos.x
                  && block.BlockPos.y - matchedBlock.BlockPos.y < 7
                  && block.BlockPos.y - matchedBlock.BlockPos.y >= 0
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = matchedBlock.BlockPos,
                BlockType = block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());

        // Bottom
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                  !matchedBlock.IsMatched3Bottom &&
                  matchedBlock.IsMatched4Bottom
                  && matchedBlock.BlockPos.x == block.BlockPos.x
                  && matchedBlock.BlockPos.y - block.BlockPos.y < 7
                  && matchedBlock.BlockPos.y - block.BlockPos.y >= 0
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = matchedBlock.BlockPos,
                BlockType =  block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());

        // TopLeft
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                  !matchedBlock.IsMatched3TopLeft &&
                  matchedBlock.IsMatched4TopLeft 
                  && (
                      (matchedBlock.BlockPos.x == block.BlockPos.x && matchedBlock.BlockPos.y  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x-1 == block.BlockPos.x && matchedBlock.BlockPos.y+1  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x-2 == block.BlockPos.x && matchedBlock.BlockPos.y+2  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x-3 == block.BlockPos.x && matchedBlock.BlockPos.y+3  == block.BlockPos.y )
                  )
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = matchedBlock.BlockPos,
                BlockType = block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());
        
        // TopRight
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                  !matchedBlock.IsMatched3TopRight &&
                  matchedBlock.IsMatched4TopRight 
                  && (
                      (matchedBlock.BlockPos.x == block.BlockPos.x && matchedBlock.BlockPos.y  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x+1 == block.BlockPos.x && matchedBlock.BlockPos.y+1  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x+2 == block.BlockPos.x && matchedBlock.BlockPos.y+2  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x+3 == block.BlockPos.x && matchedBlock.BlockPos.y+3  == block.BlockPos.y )
                  )
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = matchedBlock.BlockPos,
                BlockType = block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());
        
        // BottomLeft
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                  !matchedBlock.IsMatched3BottomLeft &&
                  matchedBlock.IsMatched4BottomLeft 
                  && (
                      (matchedBlock.BlockPos.x == block.BlockPos.x && matchedBlock.BlockPos.y  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x-1 == block.BlockPos.x && matchedBlock.BlockPos.y-1  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x-2 == block.BlockPos.x && matchedBlock.BlockPos.y-2  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x-3 == block.BlockPos.x && matchedBlock.BlockPos.y-3  == block.BlockPos.y )
                  )
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = matchedBlock.BlockPos,
                BlockType = block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());
        
        // BottomRight
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                  !matchedBlock.IsMatched3BottomRight &&
                  matchedBlock.IsMatched4BottomRight 
                  && (
                      (matchedBlock.BlockPos.x == block.BlockPos.x && matchedBlock.BlockPos.y  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x+1 == block.BlockPos.x && matchedBlock.BlockPos.y-1  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x+2 == block.BlockPos.x && matchedBlock.BlockPos.y-2  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x+3 == block.BlockPos.x && matchedBlock.BlockPos.y-3  == block.BlockPos.y )
                  )
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = matchedBlock.BlockPos,
                BlockType = block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());

        // MiddleUpperY
        /*
         *  1
         *  1
         *  0
         *  1
         */
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                  matchedBlock.IsMatched4MiddleUpperY &&
                  !matchedBlock.IsMatched3Top &&
                  !matchedBlock.IsMatched3Bottom
                  && matchedBlock.BlockPos.x == block.BlockPos.x
                  && (matchedBlock.BlockPos.y + 2 == block.BlockPos.y
                      || matchedBlock.BlockPos.y + 4 == block.BlockPos.y
                      || matchedBlock.BlockPos.y - 2 == block.BlockPos.y)
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = matchedBlock.BlockPos,
                BlockType = block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());
        
        // MiddleLowerY
        /*
         *  1
         *  0
         *  1
         *  1
         */
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                  matchedBlock.IsMatched4MiddleLowerY &&
                  !matchedBlock.IsMatched3Top &&
                  !matchedBlock.IsMatched3Bottom
                  && matchedBlock.BlockPos.x == block.BlockPos.x
                  && (matchedBlock.BlockPos.y + 2 == block.BlockPos.y
                      || matchedBlock.BlockPos.y - 2 == block.BlockPos.y
                      || matchedBlock.BlockPos.y - 4 == block.BlockPos.y)
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = matchedBlock.BlockPos,
                BlockType = block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());

        // MiddleDiagonalLowerLeftUp
        /*
         *        1
         *      0
         *    1
         *  1 
         */
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                  matchedBlock.IsMatched4MiddleDiagonalLowerLeftUp &&
                  !matchedBlock.IsMatched3MiddleDiagonalLowerLeft &&
                  !matchedBlock.IsMatched3BottomLeft
                  && (
                      (matchedBlock.BlockPos.x == block.BlockPos.x && matchedBlock.BlockPos.y  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x+1 == block.BlockPos.x && matchedBlock.BlockPos.y+1  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x-1 == block.BlockPos.x && matchedBlock.BlockPos.y-1  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x-2 == block.BlockPos.x && matchedBlock.BlockPos.y-2  == block.BlockPos.y )
                  )
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = matchedBlock.BlockPos,
                BlockType = block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList()); 
        
        
        // MiddleDiagonalLowerLeftDown
        /*
         *        1
         *      1
         *    0
         *  1 
         */
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                  matchedBlock.IsMatched4MiddleDiagonalLowerLeftDown &&
                  !matchedBlock.IsMatched3MiddleDiagonalLowerLeft &&
                  !matchedBlock.IsMatched3TopRight
                  && (
                      (matchedBlock.BlockPos.x == block.BlockPos.x && matchedBlock.BlockPos.y  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x+1 == block.BlockPos.x && matchedBlock.BlockPos.y+1  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x+2 == block.BlockPos.x && matchedBlock.BlockPos.y+2  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x-1 == block.BlockPos.x && matchedBlock.BlockPos.y-1  == block.BlockPos.y )
                  )
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = matchedBlock.BlockPos,
                BlockType = block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());

        // MiddleDiagonalLowerRightUp
        /*
         *  1
         *    0
         *      1
         *        1 
         */
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                matchedBlock.IsMatched4MiddleDiagonalLowerRightUp &&
                !matchedBlock.IsMatched3MiddleDiagonalLowerRight &&
                !matchedBlock.IsMatched3BottomRight
                && (
                    (matchedBlock.BlockPos.x == block.BlockPos.x && matchedBlock.BlockPos.y  == block.BlockPos.y ) ||
                    (matchedBlock.BlockPos.x+1 == block.BlockPos.x && matchedBlock.BlockPos.y-1  == block.BlockPos.y ) ||
                    (matchedBlock.BlockPos.x+2 == block.BlockPos.x && matchedBlock.BlockPos.y-2  == block.BlockPos.y ) ||
                    (matchedBlock.BlockPos.x-1 == block.BlockPos.x && matchedBlock.BlockPos.y+1  == block.BlockPos.y )
                )
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = matchedBlock.BlockPos,
                BlockType = block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());  
        
        
        // MiddleDiagonalLowerRightDown
        /*
         *  1
         *    1
         *      0
         *        1 
         */
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                matchedBlock.IsMatched4MiddleDiagonalLowerRightDown &&
                !matchedBlock.IsMatched3MiddleDiagonalLowerRight &&
                !matchedBlock.IsMatched3TopLeft
                && (
                    (matchedBlock.BlockPos.x == block.BlockPos.x && matchedBlock.BlockPos.y  == block.BlockPos.y ) ||
                    (matchedBlock.BlockPos.x+1 == block.BlockPos.x && matchedBlock.BlockPos.y-1  == block.BlockPos.y ) ||
                    (matchedBlock.BlockPos.x-1 == block.BlockPos.x && matchedBlock.BlockPos.y+1  == block.BlockPos.y ) ||
                    (matchedBlock.BlockPos.x-2 == block.BlockPos.x && matchedBlock.BlockPos.y+2  == block.BlockPos.y )
                )
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = matchedBlock.BlockPos,
                BlockType = block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList()); 
        
        /*
         *  매치 3 제거 블럭 추가 
         */
        
        // Top
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                !matchedBlock.IsMatched4Top &&
                  matchedBlock.IsMatched3Top
                  && matchedBlock.BlockPos.x == block.BlockPos.x
                  && block.BlockPos.y - matchedBlock.BlockPos.y < 5
                  && block.BlockPos.y - matchedBlock.BlockPos.y >= 0
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = block.BlockPos,
                BlockType =  block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());

        // Bottom
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                !matchedBlock.IsMatched4Bottom &&
                  matchedBlock.IsMatched3Bottom
                  && matchedBlock.BlockPos.x == block.BlockPos.x
                  && matchedBlock.BlockPos.y - block.BlockPos.y < 5
                  && matchedBlock.BlockPos.y - block.BlockPos.y >= 0
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = block.BlockPos,
                BlockType =  block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());

        // TopLeft
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                !matchedBlock.IsMatched4TopLeft &&
                !matchedBlock.IsMatched4MiddleDiagonalLowerRightDown && 
                  matchedBlock.IsMatched3TopLeft
                  && (
                      (matchedBlock.BlockPos.x == block.BlockPos.x && matchedBlock.BlockPos.y  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x-1 == block.BlockPos.x && matchedBlock.BlockPos.y+1  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x-2 == block.BlockPos.x && matchedBlock.BlockPos.y+2  == block.BlockPos.y )
                      )
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = block.BlockPos,
                BlockType = block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());

        // TopRight
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                !matchedBlock.IsMatched4TopRight &&
                !matchedBlock.IsMatched4MiddleDiagonalLowerLeftDown &&
                  matchedBlock.IsMatched3TopRight
                  && (
                      (matchedBlock.BlockPos.x == block.BlockPos.x && matchedBlock.BlockPos.y  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x+1 == block.BlockPos.x && matchedBlock.BlockPos.y+1  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x+2 == block.BlockPos.x && matchedBlock.BlockPos.y+2  == block.BlockPos.y )
                  )
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = block.BlockPos,
                BlockType = block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());
        
        // BottomLeft
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                !matchedBlock.IsMatched4BottomLeft &&
                !matchedBlock.IsMatched4MiddleDiagonalLowerLeftUp &&
                  matchedBlock.IsMatched3BottomLeft
                  && (
                      (matchedBlock.BlockPos.x == block.BlockPos.x && matchedBlock.BlockPos.y  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x-1 == block.BlockPos.x && matchedBlock.BlockPos.y-1  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x-2 == block.BlockPos.x && matchedBlock.BlockPos.y-2  == block.BlockPos.y )
                  )
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = block.BlockPos,
                BlockType = block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());
        
        // BottomRight
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                !matchedBlock.IsMatched4BottomRight &&
                !matchedBlock.IsMatched4MiddleDiagonalLowerRightUp &&
                  matchedBlock.IsMatched3BottomRight
                  && (
                      (matchedBlock.BlockPos.x == block.BlockPos.x && matchedBlock.BlockPos.y  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x+1 == block.BlockPos.x && matchedBlock.BlockPos.y-1  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x+2 == block.BlockPos.x && matchedBlock.BlockPos.y-2  == block.BlockPos.y )
                  )
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = block.BlockPos,
                BlockType = block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());
        
        // MiddleY
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                  matchedBlock.IsMatched3MiddleY
                  && matchedBlock.BlockPos.x == block.BlockPos.x
                  && (
                      matchedBlock.BlockPos.y  == block.BlockPos.y  ||
                       matchedBlock.BlockPos.y+2  == block.BlockPos.y ||
                       matchedBlock.BlockPos.y-2  == block.BlockPos.y
                  )
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = block.BlockPos,
                BlockType =  block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());

        // MiddleDiagonalLeftDown
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                  matchedBlock.IsMatched3MiddleDiagonalLowerLeft
                  && (
                      (matchedBlock.BlockPos.x == block.BlockPos.x && matchedBlock.BlockPos.y  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x-1 == block.BlockPos.x && matchedBlock.BlockPos.y-1  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x+1 == block.BlockPos.x && matchedBlock.BlockPos.y+1  == block.BlockPos.y )
                  )
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = block.BlockPos,
                BlockType =  block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());
        

        // MiddleDiagonalRightDown
        removeBlocks.AddRange((from block in blocks.BlocksMap.Values
            where 
                  matchedBlock.IsMatched3MiddleDiagonalLowerRight
                  && (
                      (matchedBlock.BlockPos.x == block.BlockPos.x && matchedBlock.BlockPos.y  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x+1 == block.BlockPos.x && matchedBlock.BlockPos.y-1  == block.BlockPos.y ) ||
                      (matchedBlock.BlockPos.x-1 == block.BlockPos.x && matchedBlock.BlockPos.y+1  == block.BlockPos.y )
                  )
            select new MovableBlockView()
            {
                BlockPos = block.BlockPos,
                TargetPos = block.BlockPos,
                BlockType = block.BlockType,
                BlockColor = matchedBlock.Color,
                PrevBlockType = block.BlockType,
            }).ToList());

        return removeBlocks.Distinct().ToList();
        
        /*
           !matchedBlock.IsMatched3Top &&
           !matchedBlock.IsMatched3BottomLeft &&
           !matchedBlock.IsMatched3Bottom && 
           !matchedBlock.IsMatched3TopLeft &&
           !matchedBlock.IsMatched3TopRight &&
           !matchedBlock.IsMatched3BottomRight &&
           !matchedBlock.IsMatched3MiddleDiagonalLeftDown &&
           !matchedBlock.IsMatched3MiddleY &&
         */
    }
}
