using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Core.Grid;
using Dynasty.Core.Inventory;
using Dynasty.Library;
using Dynasty.Library.Helpers;
using UnityEngine;

namespace Dynasty.Machines {

[RequireComponent(typeof(GridOutline))]
public class AutoRefiller : MachineModifier<Supply>, IStatusProvider {
    [SerializeField] InventoryAsset _inventory;
    [SerializeField] float _refillSpeed = 1f;
    [SerializeField] int _refillAmount = 1;

    GridOutline _outline;

    readonly Dictionary<Supply, State> _states = new();

    public event Action<IStatusProvider> OnStatusChanged;

    protected override void Awake() {
        base.Awake();
        _outline = GetComponent<GridOutline>();
    }

    protected override void OnEnable() {
        base.OnEnable();
        StartCoroutine(RefillLoop());
    }

    IEnumerator RefillLoop() {
        while (enabled) {
            foreach (var (supply, _) in Affected) {
                var count = Mathf.Min(_refillAmount, _inventory.GetCount(supply.RefillItem));
                if (count == 0) {
                    _states[supply] = supply.HasSupply() ? State.Low : State.Empty;
                } else {
                    _inventory.Remove(supply.RefillItem, count);
                    supply.CurrentSupply += count;
                    
                    _states[supply] = supply.HasSupply() ? State.Ok : State.Low;
                }
            }
            
            _outline.Require(Color.red, _states.Any(kvp => kvp.Value == State.Empty));
            OnStatusChanged?.Invoke(this);

            yield return CoroutineHelpers.Wait(1 / _refillSpeed);
        }
    }

    protected override void OnAdded(Supply component) {
        _states.Add(component, State.Ok);
    }
    
    protected override void OnRemoved(Supply component) {
        _states.Remove(component);
    }

    protected override bool Predicate(Supply supply) => supply.IsRefillable;


    public override IEnumerable<EntityInfo> GetInfo() {
        foreach (var info in base.GetInfo()) {
            yield return info;
        }
        
        yield return new EntityInfo("Speed", $"{_refillSpeed:0.#}");
        yield return new EntityInfo("Amount", _refillAmount.ToString());
    }

    enum State {
        Ok,
        Low,
        Empty
    }

    public IEnumerable<EntityInfo> GetStatus() {
        var shownItems = new List<ItemData>();
        
        foreach (var (supply, state) in _states) {
            if (shownItems.Contains(supply.RefillItem)) continue;
            shownItems.Add(supply.RefillItem);
            
            var (text, color) = state switch {
                State.Ok => ("Ok", Colors.Positive),
                State.Low => ("Low", Colors.Warning),
                State.Empty => ("Empty", Colors.Negative),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            yield return new EntityInfo(supply.RefillItemName, $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>");
        }
    }
}

}