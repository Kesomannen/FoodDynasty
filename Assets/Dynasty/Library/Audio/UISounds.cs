using Dynasty.Library.Classes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dynasty.Library.Audio {

public class UISounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler {
    [SerializeField] Optional<SoundEffect> _clickSound;
    [SerializeField] Optional<SoundEffect> _rightClickSound; 
    [SerializeField] Optional<SoundEffect> _hoverSound;
    [Space]
    [SerializeField] Optional<SoundEffect> _enableSound;
    [SerializeField] Optional<SoundEffect> _disableSound;
    
    void OnEnable() {
        Play(_enableSound);
    }
    
    void OnDisable() {
        Play(_disableSound);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Play(_hoverSound);
    }

    public void OnPointerClick(PointerEventData eventData) {
        Play(eventData.button == PointerEventData.InputButton.Left ? _clickSound : _rightClickSound);
    }
    
    static void Play(Optional<SoundEffect> sound) {
        if (sound.Enabled) {
            SoundManager.Instance.Play(sound.Value);
        }
    }
}

}