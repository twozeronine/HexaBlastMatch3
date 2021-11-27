using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Data;
using UnityEngine;
using Newtonsoft.Json;

public interface ILoader<TKey, TValue>
{
    Dictionary<TKey, TValue> MakeDict();
}

public class DataManager
{
    public ConstantsTableData ConstantsTableData { get; private set; } = new ConstantsTableData();
    public Dictionary<string,string> ConstantsTableDict{ get; private set; } = new Dictionary<string,string>();
    public Dictionary<int,Data.StageTableData> StageInfo{ get; private set; } = new Dictionary<int,Data.StageTableData>();
    
    public void Init()
    {
        ConstantsTableData = LoadJson<ConstantsTable, string, string>("ConstantsTable").constantsTableData;
        ConstantsTableDict = LoadJson<ConstantsTable, string, string>("ConstantsTable").MakeDict();
        StageInfo = LoadJson<StageData, int, Data.StageTableData>("StageTable").MakeDict();
        
    }
    
    private static TLoader LoadJson<TLoader, TKey, TValue>(string path) where TLoader : ILoader<TKey, TValue>
    {
        var textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonConvert.DeserializeObject<TLoader>(textAsset.text);
    }
}
