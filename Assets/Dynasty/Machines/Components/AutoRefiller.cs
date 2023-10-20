using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Core.Grid;
using Dynasty.Core.Inventory;
using Dynasty.Library;
using Dynasty.Library.Helpers;
using Dynasty.Library.Pooling;
using UnityEngine;

namespace Dynasty.Machines {

[RequireComponent(typeof(GridOutline))]
public class AutoRefiller : MachineModifier<Supply>, IStatusProvider, IBoostableProperty {
    [SerializeField] InventoryAsset _inventory;
    [SerializeField] FloatDataProperty _refillSpeed;
    [SerializeField] int _refillAmount = 1;
    [Space]
    [SerializeField] CustomObjectPool<PoolableComponent<SpriteRenderer>> _itemSpritePool;
    [SerializeField] Transform _itemStart;
    [SerializeField] float _itemSpeed;
    [SerializeField] AnimationCurve _itemEaseCurve;

    GridOutline _outline;

    readonly Dictionary<Supply, State> _states = new();

    public event Action<IStatusProvider> OnStatusChanged;

    protected override void Awake() {
        base.Awake();
        _outline = GetComponent<GridOutline>();
    }

    void OnEnable() {
        StartCoroutine(RefillLoop());
    }

    IEnumerator RefillLoop() {
        while (enabled) {
            yield return CoroutineHelpers.Wait(1 / _refillSpeed.Value);
            
            if (Affected.Count == 0) continue;
            
            var amount = _refillAmount / Affected.Count;
            foreach (var (supply, _) in Affected) {
                var count = Mathf.Min(amount, _inventory.GetCount(supply.RefillItem));
                
                if (count > 0) {
                    supply.CurrentSupply += count;
                    _inventory.Remove(supply.RefillItem, count);
                    StartCoroutine(SpawnItem(supply));
                    
                    _states[supply] = supply.HasSupply() ? State.Ok : State.Low;
                } else {
                    _states[supply] = supply.HasSupply() ? State.Low : State.Empty;
                }
            }
            
            _outline.Require(Color.red, _states.Any(kvp => kvp.Value == State.Empty));
            OnStatusChanged?.Invoke(this);
        }

        yield break;

        IEnumerator SpawnItem(Supply supply) {
            var item = _itemSpritePool.Get();
            item.Component.sprite = supply.RefillItem.Icon;
            
            var t = item.transform;
            var targetPos = supply.transform.position;
            var startPos = _itemStart.position;
            
            t.position = startPos;
            
            var time = Vector3.Distance(t.position, targetPos) / _itemSpeed;
            LeanTween.value(0, 1, time).setOnUpdate(v => {
                t.position = Vector3.Slerp(startPos, targetPos, _itemEaseCurve.Evaluate(v));
            });
            yield return CoroutineHelpers.Wait(time);
            
            item.Dispose();
        }
    }

    protected override void OnAdded(Supply component) => _states.Add(component, State.Ok);
    protected override void OnRemoved(Supply component) => _states.Remove(component);
    protected override bool Predicate(Supply supply) => supply.IsRefillable;
    
    public override IEnumerable<EntityInfo> GetInfo() {
        foreach (var info in base.GetInfo()) {
            yield return info;
        }
        
        yield return new EntityInfo("Speed", $"{_refillSpeed.Value * _refillAmount:0.#}/s");
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
                State.Ok => ("Ok", Colors.PositiveText),
                State.Low => ("Low", Colors.WarningText),
                State.Empty => ("Empty", Colors.NegativeText),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            yield return new EntityInfo(supply.RefillItemName, $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>");
        }
    }

    public FloatDataProperty BoostableProperty => _refillSpeed;
}

}