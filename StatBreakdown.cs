using System.Collections.Generic;

namespace StatSystem
{
    public sealed class StatBreakdown<TStat, TComponent>
    {
        public TStat Stat { get; }
        public float BaseValue { get; }
        public float Value { get; }
        public IReadOnlyList<StatSourceSnapshot<TStat, TComponent>> AddSources { get; }
        public IReadOnlyList<StatComponentSnapshot<TStat, TComponent>> Components { get; }

        internal StatBreakdown(
            TStat stat,
            float baseValue,
            float value,
            IReadOnlyList<StatSourceSnapshot<TStat, TComponent>> addSources,
            IReadOnlyList<StatComponentSnapshot<TStat, TComponent>> components)
        {
            Stat = stat;
            BaseValue = baseValue;
            Value = value;
            AddSources = addSources;
            Components = components;
        }
    }
}
