using System.Collections.Generic;
using System.Linq;
using Dynasty.Library;
using Dynasty.UI.Components;
using Dynasty.UI.Miscellaneous;
using TMPro;
using UnityEngine;

namespace Dynasty.UI.Tutorial {

public class TutorialManager : MonoBehaviour {
    [SerializeField] Transform _parent;
    [SerializeField] GameEvent<PopupData> _showPopupEvent;
    [SerializeField] bool _triggerOnStart;
    [Space]
    [SerializeField] TMP_Text _titleText;
    [SerializeField] TMP_Text _descriptionText;
    [SerializeField] TutorialStage[] _stages;
    [SerializeField] ContainerObjectPool<Mission> _missionPool;

    int _stageIndex = -1;
    
    readonly Dictionary<Mission, Container<Mission>> _currentMissions = new();
    
    static bool _trigger;
    
    public static void Trigger() {
        _trigger = true;
    }

    void Awake() {
        if (!_trigger && !_triggerOnStart) {
            Cleanup();
        } else {
            _trigger = false;
        }
    }

    void Start() {
        _showPopupEvent.Raise(new PopupData {
            Header = "Welcome to Food Dynasty!",
            Body = "The tutorial missions will guide you through the basics of the game.\n" +
                   "You can skip the tutorial at any time by exiting to the menu",
            Actions = new[] {
                PopupAction.Neutral("Continue")
            }
        });
        
        NextStage();
    }

    void NextStage() {
        _stageIndex++;
        
        foreach (var (mission, container) in _currentMissions) {
            Destroy(mission);
            container.Dispose();
        } 
        
        _currentMissions.Clear();
        
        if (_stageIndex >= _stages.Length) {
            _showPopupEvent.Raise(new PopupData {
                Header = "Tutorial Complete",
                Body = "You have completed the tutorial, good luck on building your food empire!",
                Actions = new[] {
                    PopupAction.Neutral("Continue")
                }
            });
            
            Cleanup();
            return;
        }
        
        var stage = _stages[_stageIndex];
        _titleText.text = stage.Title;
        _descriptionText.text = stage.Description;
        
        foreach (var template in stage.Missions) {
            var mission = Instantiate(template);
            mission.OnCompleted += MissionCompleted;
            mission.Start();

            var container = _missionPool.Get(mission, _parent, _parent.childCount);
            _currentMissions.Add(mission, container);
        }

        return;

        void MissionCompleted(Mission mission) {
            mission.OnCompleted -= MissionCompleted;

            if (_currentMissions.All(m => m.Key.IsCompleted)) {
                NextStage();
            }
        }
    }

    void Cleanup() {
        Destroy(gameObject);
        Destroy(_parent.gameObject);
    }
}

}