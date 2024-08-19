using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.StatSystem
{
    [System.Serializable]
    public class StatData : ITraitStats
    {
        [SerializeField]
        private float _value;
        [SerializeField]
        private StatSO _statReference;
        [SerializeField]
        private int _level;
        [SerializeField]
        private string _source;
        public StatSO Stat => _statReference;

        public float Value => _value;

        public string Key => _statReference.Key;

        public int Level => _level;

        public string Source => _source;

        public StatData(StatSO stat, float value, int level, string source)
        {
            _statReference = stat;
            _value = value;
            _level = level;
            _source = source;
        }
        public StatData() { }
    }
}