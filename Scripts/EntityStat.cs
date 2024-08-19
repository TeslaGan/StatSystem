using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.StatSystem
{
    public class EntityStat 
    {
        private StatSO _stat;
        private List<ITraitStats> _traitStats;

        public string Key => _stat.Key;

        public EntityStat(StatSO stat)
        {
            _stat = stat;
            _traitStats = new List<ITraitStats>();
        }

        public void AddTrait(ITraitStats trait)
        {
            _traitStats.Add(trait);
        }
        public void RemoveTrait(ITraitStats trait)
        {
            _traitStats.Remove(trait);
        }
        public int TraitCount => _traitStats.Count;
        public float Sum => _traitStats.Sum(x => x.Value);
        public IEnumerable<string> StatSource => _traitStats.Select(x => $"{x.Source}: {x.Value}");
    }
}