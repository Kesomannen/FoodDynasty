using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Dynasty.Library.Audio {

public class SoundManager : MonoBehaviour {
    public float MasterVolume {
        get => _masterVolume;
        set {
            _masterVolume = value;
            UpdateVolumes();
        }
    }

    public float MusicVolume {
        get => _musicVolume;
        set {
            _musicVolume = value;
            UpdateVolumes();
        }
    }

    public float EffectsVolume {
        get => _effectsVolume;
        set {
            _effectsVolume = value;
            UpdateVolumes();
        }
    }
    
    static SoundManager _singleton;
    
    public static SoundManager Singleton {
        get {
            if (_singleton == null) {
                _singleton = FindObjectOfType<SoundManager>();
            }
            if (_singleton == null) {
                _singleton = new GameObject("SoundManager").AddComponent<SoundManager>();
                DontDestroyOnLoad(_singleton);
            }
            return _singleton;
        }
    }
    
    readonly Dictionary<AudioSource, SoundType> _activeSources = new();
    readonly Queue<AudioSource> _inactiveSources = new();

    float _masterVolume;
    float _musicVolume;
    float _effectsVolume;

    void Awake() {
        SceneManager.sceneUnloaded += OnSceneLoaded;
    }

    void OnDestroy() {
        SceneManager.sceneUnloaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene) {
        foreach (var audioSource in _activeSources.Keys) {
            audioSource.Stop();
        }
    }
    
    void SetVolume(AudioSource source, SoundType type) {
        source.volume = type switch {
            SoundType.Effect => _effectsVolume,
            SoundType.Music => _musicVolume,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        } * _masterVolume;
    }

    void UpdateVolumes() {
        foreach (var (source, type) in _activeSources) {
            SetVolume(source, type);
        }
    }

    public void Play(SoundEffect sound) {
        StartCoroutine(PlayRoutine(sound));
    }
    
    public IEnumerator PlayRoutine(SoundEffect sound) {
        var source = GetSource(sound.Type);
        
        source.clip = sound.Clips[Random.Range(0, sound.Clips.Count)];
        source.pitch = sound.Pitch;

        source.Play();

        yield return new WaitUntil(() => !source.isPlaying);
        
        ReleaseSource(source);
    }
    
    AudioSource GetSource(SoundType type) {
        if (_inactiveSources.Count == 0) {
            CreateSource();
        }
        
        var source = _inactiveSources.Dequeue();
        _activeSources.Add(source, type);
        SetVolume(source, type);
        return source;
    }

    void ReleaseSource(AudioSource source) {
        _activeSources.Remove(source);
        _inactiveSources.Enqueue(source);
    }

    void CreateSource() {
        var source = new GameObject("AudioSource").AddComponent<AudioSource>();
        DontDestroyOnLoad(source);
        _inactiveSources.Enqueue(source);
    }
}

public enum SoundType {
    Effect,
    Music,
}

}