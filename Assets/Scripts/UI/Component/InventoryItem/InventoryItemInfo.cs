using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryItemInfo : UIComponent<InventoryItemData> {
    [Header("Text")]
    [SerializeField] TMP_Text _nameText;
    [SerializeField] TMP_Text _flavorText;
    
    [Header("Info")] 
    [SerializeField] int _infoChildIndexStart;
    [SerializeField] Transform _infoParent;
    [SerializeField] InfoContainer _infoPrefab;

    readonly List<InfoContainer> _infoContainers = new();

    public override void SetContent(InventoryItemData content) {
        if (_infoContainers.Count > 0) {
            foreach (var container in _infoContainers) {
                Destroy(container.gameObject);
            }
            _infoContainers.Clear();
        }
        
        _nameText.text = content.Name;
        _flavorText.text = content.FlavorText;

        var info = content.GetInfo().ToArray();
        for (var i = 0; i < info.Length; i++) {
            var (infoName, value) = info[i];
            
            var container = Instantiate(_infoPrefab, _infoParent);
            container.SetContent((infoName, value));
            container.transform.SetSiblingIndex(_infoChildIndexStart + i);
            
            _infoContainers.Add(container);
        }
    }
}