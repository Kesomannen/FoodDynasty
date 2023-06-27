using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryItemDataInfo : UIComponent<ItemData> {
    [SerializeField] int _infoChildIndexStart;
    [SerializeField] Transform _infoParent;
    [SerializeField] InfoContainer _infoPrefab;

    readonly List<InfoContainer> _infoContainers = new();

    public override void SetContent(ItemData content) {
        var i = 0;
        var info = content.GetInfo().ToArray();
        for (; i < info.Length; i++) {
            var (infoName, value) = info[i];

            InfoContainer container;
            if (_infoContainers.Count > i) {
                container = _infoContainers[i];
            } else {
                container = Instantiate(_infoPrefab, _infoParent);
                _infoContainers.Add(container);
            }
            
            container.SetContent((infoName, value));
            container.transform.SetSiblingIndex(_infoChildIndexStart + i);
        }
        
        for (; i < _infoContainers.Count; i++) {
            var container = _infoContainers[i];
            _infoContainers.RemoveAt(i);
            Destroy(container.gameObject);
            i--;
        }
    }
}