using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.StatSystem
{
    public interface ITrait
    {
        public string Key { get; }
        public int Level { get; }
        public string Source { get; }
    }
}