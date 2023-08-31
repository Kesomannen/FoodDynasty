using System;
using System.Collections.Generic;
using Dynasty.Library.Events;
using Dynasty.Persistent.Core;
using UnityEngine;

namespace Dynasty.UI.Menu {

public class SaveSlotController : MonoBehaviour {
    [SerializeField] SaveManager _saveManager;
    [SerializeField] SaveSlotContainer _containerPrefab;
    [SerializeField] Transform _containerParent;
    [Space] 
    [SerializeField] GameEvent<int> _loadScene;
    [SerializeField] int _gameSceneId;

    List<SaveSlotContainer> _slots;
    
    async void Start() {
        var saveSlots = await _saveManager.GetSaveSlots();
        
        _slots = new List<SaveSlotContainer>(saveSlots.Length);
        
        for (var i = 0; i < saveSlots.Length; i++) {
            var slot = saveSlots[i];
            var container = Instantiate(_containerPrefab, _containerParent);
            container.transform.SetSiblingIndex(i);
            container.SetContent(i, slot);
            
            container.OnContinue += Continue;
            container.OnDelete += Delete;
            
            _slots.Add(container);
        }
    }

    void Continue(int index) {
        _saveManager.CurrentSaveSlot = index;
        _loadScene.Raise(_gameSceneId);
    }
    
    void Delete(int index) {
        _saveManager.DeleteSlot(index);
        Destroy(_slots[index].gameObject);
        _slots.RemoveAt(index);
    }
    
    public void New() {
        _saveManager.CurrentSaveSlot = _slots.Count;
        _loadScene.Raise(_gameSceneId);
    }
}

}