using Core.StatSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatsLibrary : ScriptableObject
{
#if UNITY_EDITOR
    [SerializeField]
    private List<StatSO> _statSOs = new List<StatSO>();
    public void OnValidate()
    {
        _statCollection = null; 
        _statCollection = _statSOs.ToDictionary(x => x.Key);
        _instance = this;
    }
#endif
    private Dictionary<string, StatSO> _statCollection;

    private static StatsLibrary _instance;
    public static StatsLibrary Instance => _instance;

    public bool TryGetStat(string key, out StatSO stat)
    {
        return _statCollection.TryGetValue(key, out stat);
    }
}
