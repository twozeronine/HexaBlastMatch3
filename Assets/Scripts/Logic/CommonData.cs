using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using UnityEngine;

namespace Data
{
    public class Tiles
    {
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        public Dictionary<Vector2Int, Tile> TilesMap = new Dictionary<Vector2Int, Tile>();

        public Tile GetTile(Vector2Int targetTilePos) => TilesMap.TryGetValue(targetTilePos, out var targetTile)
            ? targetTile.IsValid ? targetTile : null
            : null;
    }

    public class Tile
    {
        public TileType Type { get; set; }
        public Vector2Int TilePos { get; set; } = new Vector2Int(-1,-1);
        public TileColor TileColor { get; set; } = TileColor.Gray;
        public bool IsValid { get; set; }
        public Block ChildBlock { get; set; } = new Block();
        public bool CanSpawnBlockTile { get; set; } = false;
    }
    public class Blocks
    {
        public Dictionary<Vector2Int, Block> BlocksMap = new Dictionary<Vector2Int, Block>();

        public Block GetBlock(Vector2Int targetTilePos) => BlocksMap.TryGetValue(targetTilePos, out var targetTile)
            ? targetTile.IsValid ? targetTile : null
            : null;
    }
    

    public class Block
    {
        public BlockColor Color { get; set; } = BlockColor.Red;
        public BlockType BlockType { get; set; } = BlockType.Empty;
        public Vector2Int BlockPos { get; set; } = new Vector2Int(-1, -1);
        public Tile ParentTile { get; set; }
        public bool IsValid;
    }

    public class MatchedBlock : IComparable<MatchedBlock>
    {
        public Vector2Int PrevBlockPos { get; set; } = new Vector2Int(-1, -1);
        public Vector2Int BlockPos { get; set; } = new Vector2Int(-1, -1);
        public BlockColor Color { get; set; }
        public BlockType BlockType { get; set; }
        public int PriorityScore { get; set; } = 0;
        public bool IsMatched3Top { get; set; } = false;
        public bool IsMatched3TopLeft { get; set; } = false;
        public bool IsMatched3TopRight { get; set; } = false;
        public bool IsMatched3Bottom { get; set; } = false;
        public bool IsMatched3BottomRight { get; set; } = false;
        
        public bool IsMatched3BottomLeft { get; set; } = false;
        public bool IsMatched3MiddleY { get; set; } = false;
        public bool IsMatched3MiddleDiagonalLowerLeft { get; set; } = false;
        public bool IsMatched3MiddleDiagonalLowerRight { get; set; } = false;
                             
        public bool IsMatched4Top { get; set; } = false;
        public bool IsMatched4TopLeft { get; set; } = false;
        public bool IsMatched4TopRight { get; set; } = false;
        public bool IsMatched4Bottom { get; set; } = false;
        public bool IsMatched4BottomRight { get; set; } = false;
        public bool IsMatched4BottomLeft { get; set; } = false;
        public bool IsMatched4MiddleUpperY { get; set; } = false;
        public bool IsMatched4MiddleLowerY { get; set; } = false;
        public bool IsMatched4MiddleDiagonalLowerLeftUp { get; set; } = false;
        public bool IsMatched4MiddleDiagonalLowerLeftDown { get; set; } = false;
        public bool IsMatched4MiddleDiagonalLowerRightUp { get; set; } = false;
        public bool IsMatched4MiddleDiagonalLowerRightDown { get; set; } = false;

        public bool IsMatched() => IsMatched3Bottom || IsMatched3TopLeft || IsMatched3TopRight || IsMatched3Top ||
                                   IsMatched3BottomRight || IsMatched3BottomLeft || IsMatched3MiddleY
                                   || IsMatched3MiddleDiagonalLowerLeft || IsMatched3MiddleDiagonalLowerRight ||
                                   IsMatched4Top || IsMatched4TopLeft || IsMatched4TopRight ||
                                   IsMatched4Bottom || IsMatched4BottomRight || IsMatched4BottomLeft ||
                                   IsMatched4MiddleUpperY || IsMatched4MiddleLowerY || IsMatched4MiddleDiagonalLowerLeftUp ||
                                   IsMatched4MiddleDiagonalLowerLeftDown || IsMatched4MiddleDiagonalLowerRightUp ||
                                   IsMatched4MiddleDiagonalLowerRightDown;
        public int GetPriorityScore()
        {
            PriorityScore += IsMatched3Top                     ? 1 : 0;
            PriorityScore += IsMatched3TopLeft                 ? 1 : 0;
            PriorityScore += IsMatched3TopRight                ? 1 : 0;
            PriorityScore += IsMatched3Bottom                  ? 1 : 0;
            PriorityScore += IsMatched3BottomRight             ? 1 : 0;
            PriorityScore += IsMatched3BottomLeft              ? 1 : 0;
            PriorityScore += IsMatched3MiddleY                 ? 1 : 0;
            PriorityScore += IsMatched3MiddleDiagonalLowerLeft  ? 1 : 0;
            PriorityScore += IsMatched3MiddleDiagonalLowerRight ? 1 : 0;

            return PriorityScore;
        }

        public int CompareTo(MatchedBlock other)
        {
            if (this.GetPriorityScore() == other.GetPriorityScore()) return 0;

            return this.GetPriorityScore() < other.GetPriorityScore() ? 1 : -1;
        }
    }
    
    public struct MovableBlockView
    {
        public Vector2Int BlockPos { get; set; }
        public Vector2Int TargetPos { get; set; }
        public BlockType BlockType { get; set; }
        public BlockColor BlockColor { get; set; }
        public BlockType PrevBlockType { get; set; }
    }
}
