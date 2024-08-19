using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.StatSystem
{
    public interface ITraitStats : ITrait
    {
        public StatSO Stat { get; }
        public float Value { get; }
    }
}