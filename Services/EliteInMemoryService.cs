using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Archery.Services
{
    // Lưu lựa chọn theo key: $"{monthYear}:{archerId}"
    public class EliteInMemoryService
    {
        private readonly ConcurrentDictionary<string, string> _choices = new();

        private string Key(string monthYear, int archerId) => $"{monthYear}:{archerId}";

        // Lưu lựa chọn (overwrite nếu chọn lại)
        public void SaveChoice(string monthYear, int archerId, string giftName)
        {
            _choices[Key(monthYear, archerId)] = giftName;
        }

        public bool TryGetChoice(string monthYear, int archerId, out string? giftName)
        {
            return _choices.TryGetValue(Key(monthYear, archerId), out giftName);
        }

        public IEnumerable<(string MonthYear, int ArcherId, string Gift)> AllChoices()
        {
            foreach (var kv in _choices)
            {
                var parts = kv.Key.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1], out var aid))
                {
                    yield return (parts[0], aid, kv.Value);
                }
            }
        }

        // Optional: reset all choices (call monthly)
        public void ResetAll() => _choices.Clear();
    }
}