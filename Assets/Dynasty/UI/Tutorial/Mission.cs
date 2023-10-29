using System;
using Dynasty.Library;
using Dynasty.Persistent;
using Dynasty.UI.Components;
using NaughtyAttributes;
using UnityEngine;

namespace Dynasty.UI.Tutorial {

public abstract class Mission : ScriptableObject {
    [ResizableTextArea]
    [SerializeField] string _description;
    [SerializeField] Lookup<ItemData> _itemLookup;
    
    float _progress;

    public float Progress {
        get => _progress;
        protected set {
            _progress = value;
            OnProgressChanged?.Invoke(this, _progress);

            if (_progress < Goal) return;
            
            IsCompleted = true;
            OnComplete();
            OnCompleted?.Invoke(this);
        }
    }

    public string Description {
        get {
            var description = _description;
            var i = 0;
            foreach (var itemData in _itemLookup) {
                description = description.Replace(
                    $"<item={i}>",
                    $"<font-weight=500>{itemData.Name}</font-weight>"
                );
                i++;
            }
            return description;
        }
    }

    public virtual string ProgressText => $"{Progress:0}/{Goal:0}";
    
    public bool IsCompleted { get; private set; }
    public abstract float Goal { get; }

    public event Action<Mission, float> OnProgressChanged;
    public event Action<Mission> OnCompleted;

    public abstract void Start();
    protected abstract void OnComplete();
}

}