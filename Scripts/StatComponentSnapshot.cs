using System.Collections.Generic;

namespace Core.StatSystem
{
    public sealed class StatComponentSnapshot<TStat, TComponent>
    {
        public TComponent Component { get; }
        public float Value { get; }
        public IReadOnlyList<StatSourceSnapshot<TStat, TComponent>> Sources { get; }

        internal StatComponentSnapshot(TComponent component, float value, IReadOnlyList<StatSourceSnapshot<TStat, TComponent>> sources)
        {
            Component = component;
            Value = value;
            Sources = sources;
        }
    }
}
