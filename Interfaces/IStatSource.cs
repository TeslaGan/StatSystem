using System;

namespace Core.StatSystem
{
    public interface IStatSource<TStat, TComponent>
    {
        event Action Invalidated;

        TStat Stat { get; }
        TComponent Component { get; }
        StatModifierKind Kind { get; }
        float Value { get; }
    }
}
