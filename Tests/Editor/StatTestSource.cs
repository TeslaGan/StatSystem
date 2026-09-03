using Core.StatSystem;

namespace Core.StatSystem.Tests.Editor
{
    internal sealed class StatTestSource : StatSource<TestStat, TestComponent>
    {
        private float _value;

        public StatTestSource(TestStat stat, StatModifierKind kind, float value, TestComponent component = default)
        {
            Stat = stat;
            Kind = kind;
            _value = value;
            Component = component;
        }

        public override TestStat Stat { get; }
        public override TestComponent Component { get; }
        public override StatModifierKind Kind { get; }
        public override float Value => _value;

        public void SetValue(float value)
        {
            _value = value;
            Invalidate();
        }
    }

    internal enum TestStat
    {
        Attack,
        Agility
    }

    internal enum TestComponent
    {
        Equipment,
        Curse
    }
}
