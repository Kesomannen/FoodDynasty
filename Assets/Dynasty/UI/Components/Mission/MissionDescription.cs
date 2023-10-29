using Dynasty.UI.Tutorial;
using TMPro;
using UnityEngine;

namespace Dynasty.UI.Components {

public class MissionDescription : UIComponent<Mission> {
    [SerializeField] TMP_Text _description;
    
    public override void SetContent(Mission content) {
        _description.text = content.Description;
    }
}

}