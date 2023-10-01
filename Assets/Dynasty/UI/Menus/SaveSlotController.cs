using System.Collections.Generic;
using System.Linq;
using Dynasty.Core.Tooltip;
using Dynasty.Library.Events;
using Dynasty.Persistent;
using Dynasty.UI.Controllers;
using UnityEngine;

namespace Dynasty.UI.Menu {

public class SaveSlotController : MonoBehaviour {
    [SerializeField] SaveManager _saveManager;
    [SerializeField] SaveSlotContainer _containerPrefab;
    [SerializeField] Transform _containerParent;
    [Space] 
    [SerializeField] GameEvent<PopupData> _showPopupEvent;
    [SerializeField] GameEvent<int> _loadScene;
    [SerializeField] int _gameSceneId;

    List<SaveSlotContainer> _slots;
    
    async void Start() {
        var saveSlots = await _saveManager.GetSaves();
        
        _slots = new List<SaveSlotContainer>(saveSlots.Length);
        
        for (var i = 0; i < saveSlots.Length; i++) {
            var slot = saveSlots[i];
            var container = Instantiate(_containerPrefab, _containerParent);
            container.transform.SetSiblingIndex(i);
            container.SetContent(slot);
            
            container.OnContinue += Continue;
            container.OnDelete += Delete;
            
            _slots.Add(container);
        }
    }

    void Continue(SaveSlotContainer slot) {
        _saveManager.CurrentSaveId = slot.Data.Id;
        _loadScene.Raise(_gameSceneId);
    }
    
    void Delete(SaveSlotContainer slot) {
        _showPopupEvent.Raise(new PopupData {
            Header = "Delete Save",
            Body = "Are you sure you want to delete this save?",
            Actions = new[] {
                PopupAction.Negative("Delete", () => {
                    _saveManager.DeleteSave(slot.Data.Id);
                    Destroy(slot.gameObject);
                    _slots.Remove(slot);
                }),
                PopupAction.Neutral("Cancel")
            }
        });
    }
    
    public void New() {
        _saveManager.CurrentSaveId = GetAvailableId();
        _loadScene.Raise(_gameSceneId);
    }

    int GetAvailableId() {
        int i;
        for (i = 0; _slots.Any(slot => slot.Data.Id == i); i++) { }
        return i;
    }
}

}