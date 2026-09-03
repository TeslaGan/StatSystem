using System;
using System.Collections.Generic;

namespace Core.StatSystem
{
    internal sealed class StatComponentRecord<TStat, TComponent>
    {
        private readonly HashSet<IStatSource<TStat, TComponent>> _percentSources =
            new(ReferenceEqualityComparer<IStatSource<TStat, TComponent>>.Instance);

        private readonly HashSet<IStatSource<TStat, TComponent>> _multiplySources =
            new(ReferenceEqualityComparer<IStatSource<TStat, TComponent>>.Instance);

        public bool IsEmpty => _percentSources.Count == 0 && _multiplySources.Count == 0;

        public bool Add(IStatSource<TStat, TComponent> source)
        {
            return source.Kind switch
            {
                StatModifierKind.Percent => _percentSources.Add(source),
                StatModifierKind.Multiply => _multiplySources.Add(source),
                _ => throw new ArgumentOutOfRangeException(nameof(source.Kind))
            };
        }

        public bool Remove(IStatSource<TStat, TComponent> source)
        {
            return source.Kind switch
            {
                StatModifierKind.Percent => _percentSources.Remove(source),
                StatModifierKind.Multiply => _multiplySources.Remove(source),
                _ => false
            };
        }

        public float GetValue()
        {
            float percent = 1f;
            float multiply = 1f;

            foreach(IStatSource<TStat, TComponent> source in _percentSources)
                percent += source.Value;

            foreach(IStatSource<TStat, TComponent> source in _multiplySources)
                multiply *= source.Value;

            return percent * multiply;
        }

        public StatComponentSnapshot<TStat, TComponent> CreateSnapshot(TComponent component)
        {
            var sources = new List<StatSourceSnapshot<TStat, TComponent>>(_percentSources.Count + _multiplySources.Count);
            float percent = 1f;
            float multiply = 1f;

            foreach(IStatSource<TStat, TComponent> source in _percentSources)
            {
                float value = source.Value;
                percent += value;
                sources.Add(new StatSourceSnapshot<TStat, TComponent>(source, value));
            }

            foreach(IStatSource<TStat, TComponent> source in _multiplySources)
            {
                float value = source.Value;
                multiply *= value;
                sources.Add(new StatSourceSnapshot<TStat, TComponent>(source, value));
            }

            return new StatComponentSnapshot<TStat, TComponent>(component, percent * multiply, sources);
        }

        public void UnsubscribeAll(Action handler)
        {
            foreach(IStatSource<TStat, TComponent> source in _percentSources)
                source.Invalidated -= handler;

            foreach(IStatSource<TStat, TComponent> source in _multiplySources)
                source.Invalidated -= handler;
        }
    }
}
