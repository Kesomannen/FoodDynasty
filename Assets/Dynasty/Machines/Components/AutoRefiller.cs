﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Grid;
using Dynasty;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Machines {

[RequireComponent(typeof(GridOutline))]
public class AutoRefiller : MachineModifier<Supply>, IStatusProvider, IBoostableProperty {
    [SerializeField] InventoryAsset _inventory;
    [SerializeField] FloatDataProperty _refillSpeed;
    [SerializeField] int _refillAmount = 1;
    [SerializeField] int _targetSupply = 50;
    [Space]
    [SerializeField] CustomObjectPool<PoolableComponent<SpriteRenderer>> _itemSpritePool;
    [SerializeField] Transform _itemStart;
    [SerializeField] float _itemSpeed;
    [SerializeField] float _itemInterval;
    [SerializeField] AnimationCurve _itemEaseCurve;

    GridOutline _outline;

    readonly Dictionary<Supply, RefillerState> _states = new();
    
    public IReadOnlyDictionary<Supply, RefillerState> States => _states;
    
    public event Action<IStatusProvider> OnStatusChanged;

    protected override void Awake() {
        base.Awake();
        _outline = GetComponent<GridOutline>();
    }

    void OnEnable() {
        StartCoroutine(RefillLoop());
    }

    protected override void OnDisable() {
        base.OnDisable();
        LeanTween.cancel(gameObject, true);
    }

    IEnumerator RefillLoop() {
        while (enabled) {
            yield return CoroutineHelpers.Wait(1 / _refillSpeed.Value);
            
            if (Affected.Count == 0) continue;

            var toRefill = Affected.Where(target => target.Component.CurrentSupply < _targetSupply).ToArray();
            var amount = Mathf.CeilToInt((float) _refillAmount / toRefill.Length);
            
            foreach (var (supply, _) in toRefill) {
                var count = Mathf.Min(
                    amount, 
                    _inventory.GetCount(supply.RefillItem),
                    _targetSupply - supply.CurrentSupply
                );

                if (count == 0) continue;
                
                supply.CurrentSupply += count;
                _inventory.Remove(supply.RefillItem, count);
                StartCoroutine(SpawnItems(supply, count));
            }
            
            foreach (var (supply, state) in _states) {
                if (supply.CurrentSupply == 0) {
                    state.Status = SupplyStatus.Empty;
                } else if (supply.CurrentSupply < _targetSupply - _refillAmount) {
                    state.Status = SupplyStatus.Low;
                } else {
                    state.Status = SupplyStatus.Ok;
                }
            }
            
            _outline.Require(Color.red, States.Any(kvp => kvp.Value.Status == SupplyStatus.Empty));
            OnStatusChanged?.Invoke(this);
        }

        yield break;

        IEnumerator SpawnItems(Supply supply, int count) {
            for (var i = 0; i < count; i++) {
                SpawnItem(supply);
                yield return CoroutineHelpers.Wait(_itemInterval);
            }
        }
        
        void SpawnItem(Supply target) {
            var item = _itemSpritePool.Get();
            item.Component.sprite = target.RefillItem.Icon;
            item.gameObject.SetActive(true);
            
            var t = item.transform;
            var startPos = _itemStart.position;
            var targetPos = target.transform.position;
            targetPos.y = startPos.y;

            t.position = startPos;

            var distance = Vector3.Distance(startPos, targetPos);
            var radius = distance / 2f;
            var time = distance / _itemSpeed;

            LeanTween.value(gameObject, 0, 1, time)
                .setOnUpdate(v => {
                    var progress = _itemEaseCurve.Evaluate(v);
                    var newPos = Vector3.Lerp(startPos, targetPos, progress);
                    newPos.y += Mathf.Sin(progress * Mathf.PI) * radius;

                    t.position = newPos; 
                }).setOnComplete(() => item.Dispose());
        }
    }

    protected override void OnAdded(Supply component) => _states.Add(component, 
        new RefillerState(component) {
            Status = SupplyStatus.Ok,
        }
    );
    
    protected override void OnRemoved(Supply component) => _states.Remove(component);
    protected override bool Predicate(Supply supply) => supply.IsRefillable;
    
    public override IEnumerable<EntityInfo> GetInfo() {
        foreach (var info in base.GetInfo()) {
            yield return info;
        }
        
        yield return new EntityInfo("Speed", $"{_refillSpeed.Value * _refillAmount:0.#}/s");
    }

    public IEnumerable<EntityInfo> GetStatus() {
        var shownItems = new List<ItemData>();
        
        foreach (var (supply, state) in States) {
            if (shownItems.Contains(supply.RefillItem)) continue;
            shownItems.Add(supply.RefillItem);
            
            yield return new EntityInfo(supply.RefillItemName, state.StatusString);
        }
    }

    public FloatDataProperty BoostableProperty => _refillSpeed;
}

public class RefillerState {
    public readonly Supply Supply;
    SupplyStatus _status;

    public SupplyStatus Status {
        get => _status;
        set {
            var previous = _status;
            _status = value;
            if (previous != _status) {
                OnChanged?.Invoke(this);
            }
        }
    }

    Color Color => Status switch {
        SupplyStatus.Ok => Colors.PositiveText,
        SupplyStatus.Low => Colors.WarningText,
        SupplyStatus.Empty => Colors.NegativeText,
        _ => throw new ArgumentOutOfRangeException()
    };
    
    public string StatusString => $"<color=#{ColorUtility.ToHtmlStringRGB(Color)}>{Status}</color>";

    public RefillerState(Supply supply) {
        Supply = supply;
    }

    public event Action<RefillerState> OnChanged;
}

public enum SupplyStatus {
    Ok,
    Low,
    Empty
}

}