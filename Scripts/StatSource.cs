using System;

namespace Core.StatSystem
{
    public abstract class StatSource<TStat, TComponent> : IStatSource<TStat, TComponent>
    {
        public event Action Invalidated;

        public abstract TStat Stat { get; }
        public abstract TComponent Component { get; }
        public abstract StatModifierKind Kind { get; }
        public abstract float Value { get; }

        protected void Invalidate()
        {
            Invalidated?.Invoke();
        }
    }
}
