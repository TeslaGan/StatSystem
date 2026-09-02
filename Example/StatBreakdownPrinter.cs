using System;
using System.Linq;
using StatSystem;

namespace Example
{
    public static class StatBreakdownPrinter
    {
        public static void Print(
            StatBreakdown<CharacterStat, CharacterStatComponent> breakdown)
        {
            Console.WriteLine($"{breakdown.Stat}: {breakdown.Value:0.##}");
            Console.WriteLine();

            Console.WriteLine("Base");

            foreach(StatSourceSnapshot<CharacterStat, CharacterStatComponent> source
                in breakdown.AddSources.OrderBy(GetSourceName))
            {
                Console.WriteLine(
                    $"  {GetSourceName(source),-24} {Format(source.Kind, source.Value),8}");
            }

            Console.WriteLine($"  {"Total",-24} {breakdown.BaseValue,8:0.##}");
            Console.WriteLine();

            foreach(StatComponentSnapshot<CharacterStat, CharacterStatComponent> component
                in breakdown.Components.OrderBy(component => component.Component))
            {
                Console.WriteLine($"[{component.Component}] x{component.Value:0.##}");

                foreach(StatSourceSnapshot<CharacterStat, CharacterStatComponent> source
                    in component.Sources.OrderBy(GetSourceName))
                {
                    Console.WriteLine(
                        $"  {GetSourceName(source),-24} {Format(source.Kind, source.Value),8}");
                }

                Console.WriteLine();
            }

            Console.WriteLine($"Result: {breakdown.Value:0.##}");
            Console.WriteLine();
        }

        private static string GetSourceName(
            StatSourceSnapshot<CharacterStat, CharacterStatComponent> source)
        {
            return source.Source.ToString();
        }

        private static string Format(StatModifierKind kind, float value)
        {
            return kind switch
            {
                StatModifierKind.Add => value.ToString("+0.##;-0.##;0"),
                StatModifierKind.Percent => value.ToString("+0.##%;-0.##%;0%"),
                StatModifierKind.Multiply => $"x{value:0.##}",
                _ => throw new ArgumentOutOfRangeException(nameof(kind))
            };
        }
    }
}
