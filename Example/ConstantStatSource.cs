using StatSystem;

namespace Example
{
    public sealed class ConstantStatSource : StatSource<CharacterStat, CharacterStatComponent>
    {
        public string Name { get; }

        public override CharacterStat Stat { get; }
        public override CharacterStatComponent Component { get; }
        public override StatModifierKind Kind { get; }
        public override float Value { get; }

        public ConstantStatSource(
            string name,
            CharacterStat stat,
            StatModifierKind kind,
            float value,
            CharacterStatComponent component = default)
        {
            Name = name;
            Stat = stat;
            Kind = kind;
            Value = value;
            Component = component;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
