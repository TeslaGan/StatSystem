using Core.StatSystem;

namespace Example
{
    public static class CharacterStatsExample
    {
        public static void Run()
        {
            using var stats = new StatContainer<CharacterStat, CharacterStatComponent>();

            stats.AddSource(new ConstantStatSource("Character", CharacterStat.Attack, StatModifierKind.Add, 10f));
            stats.AddSource(new ConstantStatSource("Character", CharacterStat.Agility, StatModifierKind.Add, 10f));

            stats.AddSource(new ConstantStatSource("Longsword", CharacterStat.Attack, StatModifierKind.Add, 5f));
            stats.AddSource(new ConstantStatSource("Sword Mastery", CharacterStat.Attack, StatModifierKind.Percent, 0.2f, CharacterStatComponent.Equipment));
            stats.AddSource(new ConstantStatSource("Cursed Ring", CharacterStat.Attack, StatModifierKind.Percent, -0.3f, CharacterStatComponent.Curse));

            stats.AddSource(new ConstantStatSource("Hamburger", CharacterStat.Attack, StatModifierKind.Multiply, 3f, CharacterStatComponent.Burger));
            stats.AddSource(new ConstantStatSource("Burger Sauce", CharacterStat.Attack, StatModifierKind.Percent, 0.5f, CharacterStatComponent.Burger));
            stats.AddSource(new ConstantStatSource("Hamburger", CharacterStat.Agility, StatModifierKind.Multiply, 0.8f, CharacterStatComponent.Burger));
            stats.AddSource(new ConstantStatSource("Burger Grease", CharacterStat.Agility, StatModifierKind.Percent, -0.1f, CharacterStatComponent.Burger));

            stats.AddSource(new ConstantStatSource("Character", CharacterStat.MoveSpeed, StatModifierKind.Add, 2f));
            stats.AddSource(new DependentStatSource("Agility", CharacterStat.MoveSpeed, CharacterStat.Agility, 0.1f, stats));

            if(stats.TryGetBreakdown(CharacterStat.Attack, out StatBreakdown<CharacterStat, CharacterStatComponent> attack))
                StatBreakdownPrinter.Print(attack);

            if(stats.TryGetBreakdown(CharacterStat.Agility, out StatBreakdown<CharacterStat, CharacterStatComponent> agility))
                StatBreakdownPrinter.Print(agility);
        }
    }
}
