using System.Collections;
using System.Collections.Generic;
using Data;
using Logic;
using UnityEngine;

public struct StageInfo
{
    public StageTableData StageTableData;
    public Dictionary<Vector2Int, Tile> StageMap;
}

public class StageManager
{
    public StageInfo GetStage(int stageId)
    {


        var stageInfo = new StageInfo()
        {
            StageTableData = Managers.Data.StageInfo[stageId],
            StageMap = DeserializedStageMap(stageId),
        };

        return stageInfo;
    }

    private static Dictionary<Vector2Int, Tile> DeserializedStageMap(int stageId)
    {
        // Json으로 된 맵 데이터를 가져옴.
        
        // 0021.json 
        
        // Deserialize(0021.json)
        
        // 타일맵 정보 불러옴 
        
        // Default TileMap Size 
        // MaxWidth : 9
        // MaxHeight : 9
        
        // Stage21  
        // Doubled coordinates
        /*    0 1 2 3 4 5 6 7 8
         *   |-------------------- | X % 2 == 0 | X % 2 == 1
         *   |   0   0   0   0     |            | 17
         *   | 0 0 0 0 0 0 0 0 0   | 16         | 15
         *   | 0 0 0 1 0 1 0 0 0   | 14         | 13
         *   | 0 1 0 1 1 1 0 1 0   | 12         | 11
         *   | 0 1 1 1 1 1 1 1 0   | 10         | 9
         *   | 0 1 1 1 1 1 1 1 0   | 8          | 7
         *   | 0 0 1 1 1 1 1 0 0   | 6          | 5
         *   | 0 0 1 0 1 0 1 0 0   | 4          | 3
         *   | 0 0 0 0 1 0 0 0 0   | 2          | 1
         *   | 0   0   0   0   0   | 0          | 
         *                   
         */

        var deSerializedStageTile = new Dictionary<Vector2Int,Tile>()
        {
            {new Vector2Int(0,0),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(0,0)}},
            {new Vector2Int(0,2),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(0,2)}},
            {new Vector2Int(0,4),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(0,4)}},
            {new Vector2Int(0,6),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(0,6)}},
            {new Vector2Int(0,8),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(0,8)}},
            {new Vector2Int(0,10),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(0,10)}},
            {new Vector2Int(0,12),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(0,12)}},
            {new Vector2Int(0,14),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(0,14)}},
            {new Vector2Int(0,16),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(0,16)}},
            {new Vector2Int(1,1),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(1,1)}},
            {new Vector2Int(1,3),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(1,3)}},
            {new Vector2Int(1,5),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(1,5)}},
            {new Vector2Int(1,7),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(1,7)}},
            {new Vector2Int(1,9),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(1,9)}},
            {new Vector2Int(1,11),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(1,11)}},
            {new Vector2Int(1,13),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(1,13)}},
            {new Vector2Int(1,15),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(1,15)}},
            {new Vector2Int(1,17),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(1,17)}},
            {new Vector2Int(2,0),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(2,0)}},
            {new Vector2Int(2,2),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(2,2)}},
            {new Vector2Int(2,4),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(2,4)}},
            {new Vector2Int(2,6),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(2,6)}},
            {new Vector2Int(2,8),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(2,8)}},
            {new Vector2Int(2,10),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(2,10)}},
            {new Vector2Int(2,12),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(2,12)}},
            {new Vector2Int(2,14),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(2,14)}},
            {new Vector2Int(2,16),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(2,16)}},
            {new Vector2Int(3,1),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(3,1)}},
            {new Vector2Int(3,3),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(3,3)}},
            {new Vector2Int(3,5),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(3,5)}},
            {new Vector2Int(3,7),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(3,7)}},
            {new Vector2Int(3,9),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(3,9)}},
            {new Vector2Int(3,11),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(3,11)}},
            {new Vector2Int(3,13),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(3,13)}},
            {new Vector2Int(3,15),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(3,15)}},
            {new Vector2Int(3,17),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(3,17)}},
            {new Vector2Int(4,0),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(4,0)}},
            {new Vector2Int(4,2),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(4,2)}},
            {new Vector2Int(4,4),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(4,4)}},
            {new Vector2Int(4,6),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(4,6)}},
            {new Vector2Int(4,8),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(4,8)}},
            {new Vector2Int(4,10),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(4,10)}},
            {new Vector2Int(4,12),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(4,12)}},
            {new Vector2Int(4,14),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(4,14),CanSpawnBlockTile = true}},
            {new Vector2Int(4,16),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(4,16)}},
            {new Vector2Int(5,1),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(5,1)}},
            {new Vector2Int(5,3),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(5,3)}},
            {new Vector2Int(5,5),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(5,5)}},
            {new Vector2Int(5,7),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(5,7)}},
            {new Vector2Int(5,9),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(5,9)}},
            {new Vector2Int(5,11),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(5,11)}},
            {new Vector2Int(5,13),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(5,13)}},
            {new Vector2Int(5,15),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(5,15)}},
            {new Vector2Int(5,17),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(5,17)}},
            {new Vector2Int(6,0),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(6,0)}},
            {new Vector2Int(6,2),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(6,2)}},
            {new Vector2Int(6,4),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(6,4)}},
            {new Vector2Int(6,6),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(6,6)}},
            {new Vector2Int(6,8),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(6,8)}},
            {new Vector2Int(6,10),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(6,10)}},
            {new Vector2Int(6,12),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(6,12)}},
            {new Vector2Int(6,14),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(6,14)}},
            {new Vector2Int(6,16),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(6,16)}},
            {new Vector2Int(7,1),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(7,1)}},
            {new Vector2Int(7,3),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(7,3)}},
            {new Vector2Int(7,5),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(7,5)}},
            {new Vector2Int(7,7),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(7,7)}},
            {new Vector2Int(7,9),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(7,9)}},
            {new Vector2Int(7,11),new Tile(){IsValid = true,Type = TileType.Normal,TilePos = new Vector2Int(7,11)}},
            {new Vector2Int(7,13),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(7,13)}},
            {new Vector2Int(7,15),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(7,15)}},
            {new Vector2Int(7,17),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(7,17)}},
            {new Vector2Int(8,0),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(8,0)}},
            {new Vector2Int(8,2),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(8,2)}},
            {new Vector2Int(8,4),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(8,4)}},
            {new Vector2Int(8,6),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(8,6)}},
            {new Vector2Int(8,8),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(8,8)}},
            {new Vector2Int(8,10),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(8,10)}},
            {new Vector2Int(8,12),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(8,12)}},
            {new Vector2Int(8,14),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(8,14)}},
            {new Vector2Int(8,16),new Tile(){IsValid = false,Type = TileType.Empty,TilePos = new Vector2Int(8,16)}},
        };

        return deSerializedStageTile;
    }
}
