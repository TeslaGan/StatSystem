using System;
using StatSystem;

namespace Example
{
    public static class CharacterStatsExample
    {
        public static void Run()
        {
            using var stats = new StatContainer<CharacterStat, CharacterStatComponent>();

            var baseAttack = new ConstantStatSource(
                "Character",
                CharacterStat.Attack,
                StatModifierKind.Add,
                10f);

            var baseAgility = new ConstantStatSource(
                "Character",
                CharacterStat.Agility,
                StatModifierKind.Add,
                10f);

            var baseMoveSpeed = new ConstantStatSource(
                "Character",
                CharacterStat.MoveSpeed,
                StatModifierKind.Add,
                2f);

            var agilityMoveSpeed = new DependentStatSource(
                "Agility",
                CharacterStat.MoveSpeed,
                CharacterStat.Agility,
                0.1f,
                stats);

            stats.AddSource(baseAttack);
            stats.AddSource(baseMoveSpeed);
            stats.AddSource(agilityMoveSpeed);

            Console.WriteLine($"Agility exists: {stats.TryGetValue(CharacterStat.Agility, out _)}");
            Console.WriteLine($"Agility: {stats.GetValue(CharacterStat.Agility):0.##}");
            Console.WriteLine($"Move speed: {stats.GetValue(CharacterStat.MoveSpeed):0.##}");
            Console.WriteLine();

            stats.AddSource(baseAgility);

            Console.WriteLine($"Agility: {stats.GetValue(CharacterStat.Agility):0.##}");
            Console.WriteLine($"Move speed: {stats.GetValue(CharacterStat.MoveSpeed):0.##}");
            Console.WriteLine();

            var sword = new ConstantStatSource(
                "Longsword",
                CharacterStat.Attack,
                StatModifierKind.Add,
                5f);

            var swordMastery = new ConstantStatSource(
                "Sword Mastery",
                CharacterStat.Attack,
                StatModifierKind.Percent,
                0.2f,
                CharacterStatComponent.Equipment);

            var cursedRing = new ConstantStatSource(
                "Cursed Ring",
                CharacterStat.Attack,
                StatModifierKind.Percent,
                -0.3f,
                CharacterStatComponent.Curse);

            var hamburgerAttack = new ConstantStatSource(
                "Hamburger",
                CharacterStat.Attack,
                StatModifierKind.Multiply,
                3f,
                CharacterStatComponent.Burger);

            var burgerSauce = new ConstantStatSource(
                "Burger Sauce",
                CharacterStat.Attack,
                StatModifierKind.Percent,
                0.5f,
                CharacterStatComponent.Burger);

            var hamburgerAgility = new ConstantStatSource(
                "Hamburger",
                CharacterStat.Agility,
                StatModifierKind.Multiply,
                0.8f,
                CharacterStatComponent.Burger);

            var burgerGrease = new ConstantStatSource(
                "Burger Grease",
                CharacterStat.Agility,
                StatModifierKind.Percent,
                -0.1f,
                CharacterStatComponent.Burger);

            stats.AddSource(sword);
            stats.AddSource(swordMastery);
            stats.AddSource(cursedRing);
            stats.AddSource(hamburgerAttack);
            stats.AddSource(burgerSauce);
            stats.AddSource(hamburgerAgility);
            stats.AddSource(burgerGrease);

            if(stats.TryGetBreakdown(
                CharacterStat.Attack,
                out StatBreakdown<CharacterStat, CharacterStatComponent> attack))
            {
                StatBreakdownPrinter.Print(attack);
            }

            if(stats.TryGetBreakdown(
                CharacterStat.Agility,
                out StatBreakdown<CharacterStat, CharacterStatComponent> agility))
            {
                StatBreakdownPrinter.Print(agility);
            }
        }
    }
}
