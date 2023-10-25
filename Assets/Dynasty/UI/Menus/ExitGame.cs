using Dynasty.Core.Tooltip;
using Dynasty.Library;
using Dynasty.UI.Controllers;
using UnityEngine;

namespace Dynasty.UI {

public class ExitGame : MonoBehaviour {
    [SerializeField] GameEvent<PopupData> _showPopupEvent;
    
    public void Exit() {
        _showPopupEvent.Raise(new PopupData {
            Header = "Exit Game",
            Body = "Are you sure you want to exit?",
            Actions = new[] {
                PopupAction.Negative("Exit", Application.Quit),
                PopupAction.Neutral("Cancel")
            }
        });
    }
}

}