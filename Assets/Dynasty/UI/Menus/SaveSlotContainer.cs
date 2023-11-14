using System;
using Dynasty.Persistent;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dynasty.UI {

public class SaveSlotContainer : MonoBehaviour, IPointerEnterHandler {
    [SerializeField] TMP_Text _name;
    [SerializeField] TMP_Text _lastPlayed;

    public SaveSlot Data { get; private set; }

    public event Action<SaveSlotContainer> OnContinue, OnDelete, OnHover;

    public void SetContent(SaveSlot saveSlot) {
        Data = saveSlot;
        
        _name.text = $"Save {saveSlot.Id + 1}";
        var timeAgo = DateTime.Now - saveSlot.LastPlayed;

        if (timeAgo.TotalDays > 1) {
            _lastPlayed.text = $"{timeAgo.Days} days ago";
        } else if (timeAgo.TotalHours > 1) {
            _lastPlayed.text = $"{timeAgo.Hours} hours ago";
        } else if (timeAgo.TotalMinutes > 1) {
            _lastPlayed.text = $"{timeAgo.Minutes} minutes ago";
        } else {
            _lastPlayed.text = "Just now";
        }
    }
    
    public void Continue() {
        OnContinue?.Invoke(this);
    }
    
    public void Delete() {
        OnDelete?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        OnHover?.Invoke(this);
    }
}

}