using System;
using System.Collections.Generic;

namespace Core.StatSystem
{
    public sealed class StatContainer<TStat, TComponent> : IStatContainer<TStat>, IDisposable
    {
        private readonly Dictionary<TStat, StatRecord<TStat, TComponent>> _records = new();
        private readonly HashSet<TStat> _evaluation = new();
        private bool _isDisposed;

        public void AddSource(IStatSource<TStat, TComponent> source)
        {
            ThrowIfDisposed();

            if(source == null)
                throw new ArgumentNullException(nameof(source));

            bool recordCreated = false;

            if(_records.TryGetValue(source.Stat, out StatRecord<TStat, TComponent> record) == false)
            {
                record = new StatRecord<TStat, TComponent>();
                _records.Add(source.Stat, record);
                recordCreated = true;
            }

            if(record.Add(source) == false)
            {
                if(recordCreated)
                    _records.Remove(source.Stat);

                throw new InvalidOperationException("Stat source is already added.");
            }

            source.Invalidated += InvalidateAll;
            InvalidateAll();
        }

        public bool RemoveSource(IStatSource<TStat, TComponent> source)
        {
            ThrowIfDisposed();

            if(source == null)
                throw new ArgumentNullException(nameof(source));

            if(_records.TryGetValue(source.Stat, out StatRecord<TStat, TComponent> record) == false)
                return false;

            if(record.Remove(source) == false)
                return false;

            source.Invalidated -= InvalidateAll;

            if(record.IsEmpty)
                _records.Remove(source.Stat);

            InvalidateAll();
            return true;
        }

        public float GetValue(TStat stat)
        {
            ThrowIfDisposed();

            return TryGetValueInternal(stat, out float value)
                ? value
                : 0f;
        }

        public bool TryGetValue(TStat stat, out float value)
        {
            ThrowIfDisposed();
            return TryGetValueInternal(stat, out value);
        }

        public bool TryGetBreakdown(TStat stat, out StatBreakdown<TStat, TComponent> breakdown)
        {
            ThrowIfDisposed();

            if(_records.TryGetValue(stat, out StatRecord<TStat, TComponent> record) == false)
            {
                breakdown = null;
                return false;
            }

            BeginEvaluation(stat);

            try
            {
                breakdown = record.CreateBreakdown(stat);
                return true;
            }
            finally
            {
                EndEvaluation(stat);
            }
        }

        public void Dispose()
        {
            if(_isDisposed)
                return;

            foreach(StatRecord<TStat, TComponent> record in _records.Values)
                record.UnsubscribeAll(InvalidateAll);

            _records.Clear();
            _evaluation.Clear();
            _isDisposed = true;
        }

        private bool TryGetValueInternal(TStat stat, out float value)
        {
            if(_records.TryGetValue(stat, out StatRecord<TStat, TComponent> record) == false)
            {
                value = 0f;
                return false;
            }

            BeginEvaluation(stat);

            try
            {
                value = record.Value;
                return true;
            }
            finally
            {
                EndEvaluation(stat);
            }
        }

        private void BeginEvaluation(TStat stat)
        {
            if(_evaluation.Add(stat) == false)
                throw new InvalidOperationException($"Cyclic stat dependency detected for '{stat}'.");
        }

        private void EndEvaluation(TStat stat)
        {
            _evaluation.Remove(stat);
        }

        private void InvalidateAll()
        {
            if(_isDisposed)
                return;

            foreach(StatRecord<TStat, TComponent> record in _records.Values)
                record.Invalidate();
        }

        private void ThrowIfDisposed()
        {
            if(_isDisposed)
                throw new ObjectDisposedException(nameof(StatContainer<TStat, TComponent>));
        }
    }
}
