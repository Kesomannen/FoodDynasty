using System;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Library {

public static class Settings {
    public static Setting<float> MasterVolume { get; }
    public static Setting<float> MusicVolume { get; }
    public static Setting<float> EffectsVolume { get; }
    
    public static Setting<float> CameraSensitivity { get; }
    
    public static Setting<QualitySetting> Quality { get; }
    public static Setting<WindowModeSetting> WindowMode { get; }
    public static Setting<MaxFramerateSetting> MaxFramerate { get; }

    static Settings() {
        MusicVolume = GetSetting("music_volume", 0.5f, value => SoundManager.Singleton.MusicVolume = value);
        EffectsVolume = GetSetting("sfx_volume", 0.5f, value => SoundManager.Singleton.EffectsVolume = value);
        MasterVolume = GetSetting("master_volume", 0.5f, value => SoundManager.Singleton.MasterVolume = value);
        
        CameraSensitivity = GetSetting("camera_sensitivity", 0.5f);
        
        Quality = GetSetting("quality", QualitySetting.Medium, value => QualitySettings.SetQualityLevel((int) value));
        
        WindowMode = GetSetting("window_mode", WindowModeSetting.Borderless, value => {
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
        
        MaxFramerate = GetSetting("max_framerate", MaxFramerateSetting.VSync, value => {
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

    static Setting<string> GetSetting(string key, string defaultValue, Action<string> onSet = null) {
        return new Setting<string>(key, defaultValue, PlayerPrefs.SetString, PlayerPrefs.GetString, onSet);
    }
    
    static Setting<float> GetSetting(string key, float defaultValue, Action<float> onSet = null) {
        return new Setting<float>(key, defaultValue, PlayerPrefs.SetFloat, PlayerPrefs.GetFloat, onSet);
    }
    
    static Setting<int> GetSetting(string key, int defaultValue, Action<int> onSet = null) {
        return new Setting<int>(key, defaultValue, PlayerPrefs.SetInt, PlayerPrefs.GetInt, onSet);
    }

    static Setting<T> GetSetting<T>(string key, T defaultValue, Action<T> onSet = null) where T : Enum {
        return new Setting<T>(
            key, defaultValue,
            (k, v) => PlayerPrefs.SetInt(k, (int) (object) v),
            k => (T) (object) PlayerPrefs.GetInt(k, (int) (object) defaultValue),
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