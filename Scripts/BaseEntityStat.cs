using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.StatSystem;
using SerializeReferenceEditor;

public class BaseEntityStat : MonoBehaviour
{
    [SerializeReference, SR]
    private List<ITraitStats> _traitStats = new();
    [SerializeField]
    private EntityStatsCollection _entityStats;

    public void Start()
    {
        _entityStats = GetComponent<EntityStatsCollection>();
        _traitStats.ForEach(x => _entityStats.AddStat(x));
    }
}
