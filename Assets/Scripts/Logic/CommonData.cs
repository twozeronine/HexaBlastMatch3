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
    }
}
