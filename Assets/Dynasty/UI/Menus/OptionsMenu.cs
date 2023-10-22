using System;
using Dynasty.Library;
using Dynasty.Library.Events;
using Dynasty.UI.Miscellanious;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.UI;

namespace Dynasty.UI.Menu {

public class OptionsMenu : MonoBehaviour {
    [SerializeField] float _timeScale;
    [SerializeField] GenericGameEvent _toggleMenu;
    [Space] 
    [SerializeField] EnumDropdown<QualitySetting> _qualityDropdown;
    [SerializeField] EnumDropdown<WindowModeSetting> _windowDropdown;
    [SerializeField] EnumDropdown<MaxFramerateSetting> _framerateDropdown;
    [Space]
    [SerializeField] Slider _masterVolumeSlider;
    [SerializeField] Slider _musicVolumeSlider;
    [SerializeField] Slider _effectsVolumeSlider;
    [Space] 
    [SerializeField] Slider _cameraSensitivitySlider;
    [SerializeField] Transform _rebindParent;
    [SerializeField] RebindActionUI _rebindPrefab;
    [SerializeField] GameObject _rebindOverlay;
    [SerializeField] TMP_Text _rebindText;
    [SerializeField] InputEvent[] _rebindableInputs;

    void Start() {
        gameObject.SetActive(false);
        _toggleMenu.AddListener(Toggle);
        
        Bind(_qualityDropdown, Settings.Quality);
        Bind(_windowDropdown, Settings.WindowMode);
        Bind(_framerateDropdown, Settings.MaxFramerate);
        
        Bind(_masterVolumeSlider, Settings.MasterVolume, 100);
        Bind(_musicVolumeSlider, Settings.MusicVolume, 100);
        Bind(_effectsVolumeSlider, Settings.EffectsVolume, 100);
        
        Bind(_cameraSensitivitySlider, Settings.CameraSensitivity, 100);
        
        foreach (var input in _rebindableInputs) {
            var rebind = Instantiate(_rebindPrefab, _rebindParent);
            
            rebind.actionReference = input.Reference;
            rebind.bindingId = input.Action.bindings[0].id.ToString();
            
            rebind.rebindOverlay = _rebindOverlay;
            rebind.rebindPrompt = _rebindText;
        }
    }

    void OnEnable() {
        Time.timeScale = _timeScale;
    }
    
    void OnDisable() {
        Time.timeScale = 1f;
    }

    void OnDestroy() {
        _toggleMenu.RemoveListener(Toggle);
        PlayerPrefs.Save();
    }
    
    void Toggle() {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    static void Bind<T>(EnumDropdown<T> dropdown, Setting<T> setting) where T : Enum {
        dropdown.Value = setting.Value;
        dropdown.OnValueChanged += value => setting.Value = value;
    } 
    
    static void Bind(Slider slider, Setting<float> setting, float multiplier = 1f) {
        slider.value = setting.Value * multiplier;
        slider.onValueChanged.AddListener(value => setting.Value = value / multiplier);
    }
}

}