using System;
using System.Collections.Generic;
using Dynasty.Library.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dynasty.UI.Displays {

public class PopupDisplay : MonoBehaviour {
    [SerializeField] GameObject _popup;
    [SerializeField] TMP_Text _headerText;
    [SerializeField] TMP_Text _bodyText;
    [SerializeField] Transform _actionParent;
    [SerializeField] Button _actionPrefab;
    [Space]
    [SerializeField] GameEvent<PopupData> _showPopupEvent;

    readonly List<Button> _actions = new();
    
    void OnEnable() {
        _showPopupEvent.AddListener(ShowPopup);
    }
    
    void OnDisable() {
        _showPopupEvent.RemoveListener(ShowPopup);
    }

    void ShowPopup(PopupData data) {
        ConfigureText(_headerText, data.Header);
        ConfigureText(_bodyText, data.Body);

        var i = 0;
        for (; i < data.Actions.Length; i++) {
            var action = data.Actions[i];
            
            Button button;
            if (_actions.Count > i) {
                button = _actions[i];
                button.onClick.RemoveAllListeners();
            } else {
                button = Instantiate(_actionPrefab, _actionParent);
                _actions.Add(button);
            }
            
            button.GetComponentInChildren<TMP_Text>().text = action.Label;
            button.targetGraphic.color = action.Color; 
            button.onClick.AddListener(() => {
                action.Action?.Invoke();
                _popup.SetActive(false);
            });
        }

        while (_actions.Count > i) {
            var action = _actions[i];
            _actions.RemoveAt(i);
            Destroy(action.gameObject);
        }
        
        _popup.SetActive(true);
        
        return;

        void ConfigureText(TMP_Text text, string content) {
            text.text = content;
            text.gameObject.SetActive(!string.IsNullOrEmpty(content));
        }
    }
}

}