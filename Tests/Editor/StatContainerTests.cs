using System;
using NUnit.Framework;

namespace Core.StatSystem.Tests.Editor
{
    internal sealed class StatContainerTests
    {
        [Test]
        public void GetValue_MissingStat_ReturnsZero()
        {
            using var stats = new StatContainer<TestStat, TestComponent>();

            Assert.That(stats.GetValue(TestStat.Attack), Is.EqualTo(0f));
            Assert.That(stats.TryGetValue(TestStat.Attack, out _), Is.False);
        }

        [Test]
        public void GetValue_CalculatesBaseAndComponents()
        {
            using var stats = new StatContainer<TestStat, TestComponent>();

            stats.AddSource(new StatTestSource(TestStat.Attack, StatModifierKind.Add, 10f));
            stats.AddSource(new StatTestSource(TestStat.Attack, StatModifierKind.Add, 5f));
            stats.AddSource(new StatTestSource(TestStat.Attack, StatModifierKind.Percent, 0.2f, TestComponent.Equipment));
            stats.AddSource(new StatTestSource(TestStat.Attack, StatModifierKind.Percent, -0.3f, TestComponent.Curse));

            Assert.That(stats.GetValue(TestStat.Attack), Is.EqualTo(12.6f).Within(0.0001f));
        }

        [Test]
        public void Invalidated_RecalculatesCachedValue()
        {
            using var stats = new StatContainer<TestStat, TestComponent>();
            var source = new StatTestSource(TestStat.Attack, StatModifierKind.Add, 10f);

            stats.AddSource(source);
            Assert.That(stats.GetValue(TestStat.Attack), Is.EqualTo(10f));

            source.SetValue(20f);

            Assert.That(stats.GetValue(TestStat.Attack), Is.EqualTo(20f));
        }

        [Test]
        public void AddSource_SameInstanceTwice_ThrowsInvalidOperationException()
        {
            using var stats = new StatContainer<TestStat, TestComponent>();
            var source = new StatTestSource(TestStat.Attack, StatModifierKind.Add, 10f);

            stats.AddSource(source);

            Assert.Throws<InvalidOperationException>(() => stats.AddSource(source));
        }

        [Test]
        public void RemoveSource_LastSource_RemovesStat()
        {
            using var stats = new StatContainer<TestStat, TestComponent>();
            var source = new StatTestSource(TestStat.Attack, StatModifierKind.Add, 10f);

            stats.AddSource(source);
            stats.RemoveSource(source);

            Assert.That(stats.TryGetValue(TestStat.Attack, out _), Is.False);
        }

        [Test]
        public void TryGetBreakdown_ReturnsCalculationSnapshot()
        {
            using var stats = new StatContainer<TestStat, TestComponent>();
            stats.AddSource(new StatTestSource(TestStat.Attack, StatModifierKind.Add, 10f));
            stats.AddSource(new StatTestSource(TestStat.Attack, StatModifierKind.Multiply, 2f, TestComponent.Equipment));

            bool result = stats.TryGetBreakdown(TestStat.Attack, out StatBreakdown<TestStat, TestComponent> breakdown);

            Assert.That(result, Is.True);
            Assert.That(breakdown.BaseValue, Is.EqualTo(10f));
            Assert.That(breakdown.Value, Is.EqualTo(20f));
            Assert.That(breakdown.AddSources.Count, Is.EqualTo(1));
            Assert.That(breakdown.Components.Count, Is.EqualTo(1));
        }
    }
}
