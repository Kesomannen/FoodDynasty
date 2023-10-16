using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dynasty.Food {

[CreateAssetMenu(menuName = "Manager/Food Trait Database")]
public class FoodTraitDatabase : ScriptableObject {
    [SerializeField] List<Entry> _entries;
    
    static FoodTraitDatabase _singleton;
    
    public static FoodTraitDatabase Singleton {
        get {
            if (_singleton == null) {
                _singleton = Resources.Load<FoodTraitDatabase>("Food Trait Database");
            }

            return _singleton;
        }
    }

    public IReadOnlyList<Entry> Entries => _entries;
    
    public Entry AddEntry(string entryName, FoodTraitType type) {
        var hash = entryName.GetHashCode();
        if (_entries.All(entry => entry.Hash != hash)) {
            var entry = new Entry(entryName, type);
            _entries.Add(entry);
            return entry;
        }
        
        Debug.LogWarning($"Entry {entryName} already exists in database.");
        return default;
    }
    
    public void SetType(int hash, FoodTraitType type) {
        for (var i = 0; i < _entries.Count; i++) {
            var entry = _entries[i];
            if (entry.Hash != hash) continue;
            
            entry.Type = type;
            _entries[i] = entry;
        }
    }
    
    public void RemoveEntry(int hash) {
        for (var i = 0; i < _entries.Count; i++) {
            if (_entries[i].Hash != hash) continue;
            
            _entries.RemoveAt(i);
            return;
        }
    }

    public bool TryGetEntry(int hash, out Entry found) {
        foreach (var entry in _entries.Where(entry => entry.Hash == hash)) {
            found = entry;
            return true;
        }
        
        found = default;
        return false;
    }
    
    [Serializable]
    public struct Entry {
        public string Name;
        public int Hash;
        public FoodTraitType Type;
    
        public Entry(string name, FoodTraitType type) {
            Name = name;
            Hash = name.GetHashCode();
            Type = type;
        }
    
        public bool Equals(Entry other) {
            return Hash == other.Hash;
        }

        public override bool Equals(object obj) {
            return obj is Entry other && Equals(other);
        }

        public override int GetHashCode() {
            return Hash;
        }
    }
}

}