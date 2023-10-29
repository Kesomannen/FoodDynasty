using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dynasty.UI.Tutorial {

[Serializable]
public class TutorialStage {
    [SerializeField] string _title;
    [SerializeField] string _description;
    [SerializeField] Mission[] _missions;
    
    public string Title => _title;
    public string Description => _description;
    public IReadOnlyList<Mission> Missions => _missions;
}

}