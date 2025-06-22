using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, Data.Player> PlayerDict { get; private set; } = new Dictionary<int, Data.Player>();
    public Dictionary<int, Data.GeneralEnemy> EnemyDict { get; private set; } = new Dictionary<int, Data.GeneralEnemy>();
    public Dictionary<int, Data.BossEnemy> BossDict { get; private set; } = new Dictionary<int, Data.BossEnemy>();

    public void Init()
    {
        PlayerDict = LoadJson<Data.PlayerData, int, Data.Player>("StatData").MakeDict();
        EnemyDict = LoadJson<Data.GeneralEnemyData, int, Data.GeneralEnemy>("StatData").MakeDict();
        BossDict = LoadJson<Data.BossEnemyData, int, Data.BossEnemy>("StatData").MakeDict();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}
