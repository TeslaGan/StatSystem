using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.StatExpressionParser;
using System.Linq.Expressions;
using System;

namespace Core.StatSystem
{
    [CreateAssetMenu(menuName = "StatSO")]
    public class StatSO : ScriptableObject, IStats
    {
        [SerializeField]
        private string _key;
        [SerializeField]
        private string _name;
        [SerializeField, TextArea]
        private string _description;
        /* ----Future function----
        [SerializeField]
        private string _formula;
        [SerializeField]
        private Func<EntityStatsCollection,float> _lambda;
        */
        [SerializeField]
        private float _defaultValue;
        public string Key => _key;

        public string Name => _name;

        public string Description => _description;

        public float Value(EntityStatsCollection statData) 
        {
            return statData.TryGetStat(Key, out EntityStat data)?data.Sum:_defaultValue;
        }

        public float DefaultValue => _defaultValue;

    }
}