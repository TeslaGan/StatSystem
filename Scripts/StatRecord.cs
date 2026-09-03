using System;
using System.Collections.Generic;

namespace Core.StatSystem
{
    internal sealed class StatRecord<TStat, TComponent>
    {
        private readonly HashSet<IStatSource<TStat, TComponent>> _addSources =
            new(ReferenceEqualityComparer<IStatSource<TStat, TComponent>>.Instance);

        private readonly Dictionary<TComponent, StatComponentRecord<TStat, TComponent>> _components = new();
        private float _cachedValue;
        private bool _isDirty = true;

        public bool IsEmpty => _addSources.Count == 0 && _components.Count == 0;

        public float Value
        {
            get
            {
                if(_isDirty == false)
                    return _cachedValue;

                _cachedValue = CalculateValue();
                _isDirty = false;

                return _cachedValue;
            }
        }

        public bool Add(IStatSource<TStat, TComponent> source)
        {
            if(source.Kind == StatModifierKind.Add)
                return _addSources.Add(source);

            if(_components.TryGetValue(source.Component, out StatComponentRecord<TStat, TComponent> component) == false)
            {
                component = new StatComponentRecord<TStat, TComponent>();
                _components.Add(source.Component, component);
            }

            return component.Add(source);
        }

        public bool Remove(IStatSource<TStat, TComponent> source)
        {
            if(source.Kind == StatModifierKind.Add)
                return _addSources.Remove(source);

            if(_components.TryGetValue(source.Component, out StatComponentRecord<TStat, TComponent> component) == false)
                return false;

            if(component.Remove(source) == false)
                return false;

            if(component.IsEmpty)
                _components.Remove(source.Component);

            return true;
        }

        public void Invalidate()
        {
            _isDirty = true;
        }

        public StatBreakdown<TStat, TComponent> CreateBreakdown(TStat stat)
        {
            var addSources = new List<StatSourceSnapshot<TStat, TComponent>>(_addSources.Count);
            var components = new List<StatComponentSnapshot<TStat, TComponent>>(_components.Count);
            float baseValue = 0f;

            foreach(IStatSource<TStat, TComponent> source in _addSources)
            {
                float sourceValue = source.Value;
                baseValue += sourceValue;
                addSources.Add(new StatSourceSnapshot<TStat, TComponent>(source, sourceValue));
            }

            float value = baseValue;

            foreach(KeyValuePair<TComponent, StatComponentRecord<TStat, TComponent>> pair in _components)
            {
                StatComponentSnapshot<TStat, TComponent> component = pair.Value.CreateSnapshot(pair.Key);
                components.Add(component);
                value *= component.Value;
            }

            _cachedValue = value;
            _isDirty = false;

            return new StatBreakdown<TStat, TComponent>(stat, baseValue, value, addSources, components);
        }

        public void UnsubscribeAll(Action handler)
        {
            foreach(IStatSource<TStat, TComponent> source in _addSources)
                source.Invalidated -= handler;

            foreach(StatComponentRecord<TStat, TComponent> component in _components.Values)
                component.UnsubscribeAll(handler);
        }

        private float CalculateValue()
        {
            float value = 0f;

            foreach(IStatSource<TStat, TComponent> source in _addSources)
                value += source.Value;

            foreach(StatComponentRecord<TStat, TComponent> component in _components.Values)
                value *= component.GetValue();

            return value;
        }
    }
}
