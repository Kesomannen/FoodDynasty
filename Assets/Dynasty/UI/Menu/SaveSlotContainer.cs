using System;
using System.Globalization;
using Dynasty.Persistent.Core;
using TMPro;
using UnityEngine;

namespace Dynasty.UI.Menu {

public class SaveSlotContainer : MonoBehaviour {
    [SerializeField] TMP_Text _name;
    [SerializeField] TMP_Text _lastPlayed;
    
    public int Index { get; private set; }

    public event Action<int> OnContinue, OnDelete;

    public void SetContent(int index, SaveSlot saveSlot) {
        Index = index;

        _name.text = $"Save {index + 1}";
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
        OnContinue?.Invoke(Index);
    }
    
    public void Delete() {
        OnDelete?.Invoke(Index);
    }
}

}