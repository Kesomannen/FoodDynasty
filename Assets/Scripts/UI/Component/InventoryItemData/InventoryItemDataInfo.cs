using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryItemDataInfo : UIComponent<InventoryItemData> {
    [Header("Text")]
    [SerializeField] TMP_Text _nameText;
    [SerializeField] TMP_Text _flavorText;
    
    [Header("Info")] 
    [SerializeField] int _infoChildIndexStart;
    [SerializeField] Transform _infoParent;
    [SerializeField] InfoContainer _infoPrefab;

    readonly List<InfoContainer> _infoContainers = new();

    public override void SetContent(InventoryItemData content) {
        _nameText.text = content.Name;
        _flavorText.text = content.FlavorText;

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