using StatSystem;

namespace Example
{
    public sealed class DependentStatSource : StatSource<CharacterStat, CharacterStatComponent>
    {
        private readonly IStatContainer<CharacterStat> _stats;
        private readonly CharacterStat _dependency;
        private readonly float _modifier;

        public string Name { get; }

        public override CharacterStat Stat { get; }
        public override CharacterStatComponent Component => default;
        public override StatModifierKind Kind => StatModifierKind.Add;
        public override float Value => _stats.GetValue(_dependency) * _modifier;

        public DependentStatSource(
            string name,
            CharacterStat stat,
            CharacterStat dependency,
            float modifier,
            IStatContainer<CharacterStat> stats)
        {
            Name = name;
            Stat = stat;
            _dependency = dependency;
            _modifier = modifier;
            _stats = stats;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
