using System;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Library {

public static class Settings {
    public static Setting<float> MasterVolume { get; }
    public static Setting<float> MusicVolume { get; }
    public static Setting<float> EffectsVolume { get; }
    
    public static Setting<float> CameraSensitivity { get; }
    public static Setting<float> CameraShakeIntensity { get; }
    
    public static Setting<QualitySetting> Quality { get; }
    public static Setting<WindowModeSetting> WindowMode { get; }
    public static Setting<MaxFramerateSetting> MaxFramerate { get; }

    static Settings() {
        MusicVolume = SettingsFactory.Get("music_volume", 0.5f, value => SoundManager.Singleton.MusicVolume = value);
        EffectsVolume = SettingsFactory.Get("sfx_volume", 0.5f, value => SoundManager.Singleton.EffectsVolume = value);
        MasterVolume = SettingsFactory.Get("master_volume", 0.5f, value => SoundManager.Singleton.MasterVolume = value);
        
        CameraSensitivity = SettingsFactory.Get("camera_sensitivity", 0.5f);
        CameraShakeIntensity = SettingsFactory.Get("camera_shake_intensity", 1f);
        
        Quality = SettingsFactory.Get("quality", QualitySetting.Medium, value => QualitySettings.SetQualityLevel((int) value));
         
        WindowMode = SettingsFactory.Get("window_mode", WindowModeSetting.Borderless, value => {
            Screen.fullScreenMode = value switch {
                WindowModeSetting.Windowed => FullScreenMode.Windowed,
                WindowModeSetting.Borderless => FullScreenMode.FullScreenWindow,
#if UNITY_STANDALONE_WIN
                WindowModeSetting.Fullscreen => FullScreenMode.ExclusiveFullScreen,
#endif
#if UNITY_STANDALONE_OSX
                WindowModeSetting.Maximized => FullScreenMode.MaximizedWindow,
#endif
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        });
        
        MaxFramerate = SettingsFactory.Get("max_framerate", MaxFramerateSetting.VSync, value => {
            QualitySettings.vSyncCount = value == MaxFramerateSetting.VSync ? 1 : 0;
            Application.targetFrameRate = value switch {
                MaxFramerateSetting.VSync => -1,
                MaxFramerateSetting.Sixty => 60,
                MaxFramerateSetting.OneTwenty => 120,
                MaxFramerateSetting.OneSixtyFive => 165,
                MaxFramerateSetting.TwoHundredForty => 240,
                MaxFramerateSetting.Unlimited => 9999,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        });
    }
}

public static class SettingsFactory {
    public static Setting<string> Get(string key, string defaultValue = default, Action<string> onSet = null) {
        return new Setting<string>(key, defaultValue, PlayerPrefs.SetString, PlayerPrefs.GetString, onSet);
    }
    
    public static Setting<float> Get(string key, float defaultValue = default, Action<float> onSet = null) {
        return new Setting<float>(key, defaultValue, PlayerPrefs.SetFloat, PlayerPrefs.GetFloat, onSet);
    }
    
    public static Setting<int> Get(string key, int defaultValue = default, Action<int> onSet = null) {
        return new Setting<int>(key, defaultValue, PlayerPrefs.SetInt, PlayerPrefs.GetInt, onSet);
    }

    public static Setting<T> Get<T>(string key, T defaultValue = default, Action<T> onSet = null) where T : Enum {
        return new Setting<T>(
            key, defaultValue,
            (k, v) => PlayerPrefs.SetInt(k, v.ToInt()),
            k => PlayerPrefs.GetInt(k, defaultValue.ToInt()).ToEnum<T>(),
            onSet
        );
    }
}

public class Setting<T> {
    readonly string _key;
    readonly Action<string, T> _setter;
    readonly Func<string, T> _getter;
    readonly Action<T> _onSet;
        
    public T Value {
        get => _getter(_key);
        set {
            _setter(_key, value);
            _onSet?.Invoke(value); 
        }
    }
        
    public Setting(string key, T defaultValue, Action<string, T> setter, Func<string, T> getter, Action<T> onSet = null) {
        _key = key;
            
        _setter = setter;
        _getter = getter;
        _onSet = onSet;
            
        if (!PlayerPrefs.HasKey(key)) {
            Value = defaultValue;
        }
            
        _onSet?.Invoke(Value);
    }
}

public enum QualitySetting {
    Low,
    Medium,
    High,
}
    
public enum WindowModeSetting {
#if UNITY_STANDALONE_WIN
    Fullscreen,
#endif
#if UNITY_STANDALONE_OSX
    Maximized,
#endif
    Windowed,
    Borderless
}

public enum MaxFramerateSetting {
    VSync,
    Sixty,
    OneTwenty,
    OneSixtyFive,
    TwoHundredForty,
    Unlimited
}

}