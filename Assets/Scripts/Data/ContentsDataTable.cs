using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Data
{
    #region ConstansDataTable

    [Serializable]
    public class ConstantsTableData
    {
        public float BlockDefaultMoveSpeed { get; set; }
        public float BlockGravityMoveSpeed { get; set; }
        public float BlankToFullBlockTime { get; set; }
        public float TileDefaultScale { get; set; }
        public float BlockMixMoveSpeed { get; set; }
        public float DefaultHexPositionOffsetX { get; set; }
        public float DefaultHexPositionOffsetY { get; set; }
        public float BlockDefaultScale { get; set; }
        public float DefaultBlockShuffleSpeed { get; set; }
    }

    [Serializable]
    public class ConstantsTable : ILoader<string, string>
    {
        public ConstantsTableData constantsTableData = new ConstantsTableData();

        public Dictionary<string, string> MakeDict() => constantsTableData.MakeClassToDict<ConstantsTableData, string>();
    }

    #endregion

    #region Stage

    [Serializable]
    public class StageTableData
    {
        public int Id { get; set; }
        public int MoveCount { get; set; }
        public string MissionType { get; set; }
        public int MissionCount { get; set; }
    }

    [Serializable]
    public class StageData : ILoader<int, StageTableData>
    {
        public List<StageTableData> stageTableData = new List<StageTableData>();

        public Dictionary<int, StageTableData> MakeDict()
        {
            return stageTableData.ToDictionary(stageInfo => stageInfo.Id);
        }
    }

    #endregion
}