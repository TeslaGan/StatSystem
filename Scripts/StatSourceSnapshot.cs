namespace Core.StatSystem
{
    public readonly struct StatSourceSnapshot<TStat, TComponent>
    {
        public IStatSource<TStat, TComponent> Source { get; }
        public TStat Stat { get; }
        public TComponent Component { get; }
        public StatModifierKind Kind { get; }
        public float Value { get; }

        internal StatSourceSnapshot(IStatSource<TStat, TComponent> source, float value)
        {
            Source = source;
            Stat = source.Stat;
            Component = source.Component;
            Kind = source.Kind;
            Value = value;
        }
    }
}
