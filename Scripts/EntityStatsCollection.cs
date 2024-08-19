using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.StatSystem
{
    public class EntityStatsCollection : MonoBehaviour
    {
        private Dictionary<string, EntityStat> _stats = new();

        public bool TryGetStat(string key, out EntityStat data)
        {
            return _stats.TryGetValue(key, out data);
        }

        public bool TryGetStatValue(string key, out float value)
        {
            value = _stats.TryGetValue(key, out EntityStat data) ? data.Sum:0f;
            return _stats.ContainsKey(key);
        }

        public void AddStat(ITraitStats stat)
        {
            if(_stats.ContainsKey(stat.Key) == false)
            {
                _stats.Add(stat.Key, new EntityStat(stat.Stat));
            }
            _stats[stat.Key].AddTrait(stat);
        }
        public void RemoveStat(ITraitStats stat)
        {
            _stats[stat.Key].RemoveTrait(stat);
            if(_stats[stat.Key].TraitCount == 0)
                _stats.Remove(stat.Key);
        }
#if UNITY_EDITOR
        [ContextMenu("Show stat log")]
        public void ShowStats()
        {
            foreach (EntityStat stat in _stats.Values)
            {
                Debug.Log($"{stat.Key}: {stat.Sum}");
                foreach(string log in stat.StatSource)
                    Debug.Log(log);
            }
        }
#endif
    }
}