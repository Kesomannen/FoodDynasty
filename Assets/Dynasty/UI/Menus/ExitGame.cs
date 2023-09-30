using Dynasty.Library.Events;
using Dynasty.UI.Displays;
using UnityEngine;

namespace Dynasty.UI.Menu {

public class ExitGame : MonoBehaviour {
    [SerializeField] GameEvent<PopupData> _showPopupEvent;
    
    public void Exit() {
        _showPopupEvent.Raise(new PopupData {
            Header = "Exit Game",
            Body = "Are you sure you want to exit the game?",
            Actions = new[] {
                PopupAction.Confirm("Exit", Application.Quit),
                PopupAction.Cancel()
            }
        });
    }
}

}