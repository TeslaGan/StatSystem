using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.StatSystem
{
    public interface IStats
    {
        public string Key { get; }
        public string Name { get; }
        public string Description { get; }
        public float Value(EntityStatsCollection statData);
        public float DefaultValue { get; }
    }
}