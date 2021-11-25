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
        public float TileAndBlockDefaultScale { get; set; }
        public float BlockMixMoveSpeed { get; set; }
    }

    [Serializable]
    public class ConstantsTable : ILoader<string, string>
    {
        public ConstantsTableData constantsTableData = new ConstantsTableData();

        public Dictionary<string, string> MakeDict() => constantsTableData.MakeClassToDict<ConstantsTableData, string>();
    }

    #endregion

    #region Stat

    [Serializable]
    public class Stat
    {
        public int level;
        public int maxHp;
        public int attack;
        public int totalExp;
    }

    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        public List<Stat> stats = new List<Stat>();

        public Dictionary<int, Stat> MakeDict()
        {
            return stats.ToDictionary(stat => stat.level);
        }
    }

    #endregion
}