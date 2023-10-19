using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Library.Classes;
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
    
    readonly Dictionary<AudioSource, SoundEffect> _activeSources = new();
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
        foreach (var audioSource in _activeSources.Keys.ToArray()) {
            Release(audioSource);
        }
    }
    
    void SetVolume(AudioSource source, SoundEffect effect) {
        source.volume = effect.Type switch {
            SoundType.Effect => _effectsVolume,
            SoundType.Music => _musicVolume,
            _ => throw new ArgumentOutOfRangeException(nameof(effect), effect, null)
        } * _masterVolume * effect.Volume;
    }

    void UpdateVolumes() {
        foreach (var (source, type) in _activeSources) {
            SetVolume(source, type);
        }
    }
    
    public CoroutineHandle Play(SoundEffect sound, out Action cancel) {
        var source = GetSource(sound);
        cancel = () => Release(source);
        return new CoroutineHandle(this, PlayRoutine(sound, source));
    }

    public void Play(SoundEffect sound) {
        StartCoroutine(PlayRoutine(sound));
    }

    IEnumerator PlayRoutine(SoundEffect sound, AudioSource source) {
        source.clip = sound.Clips[Random.Range(0, sound.Clips.Count)];
        source.pitch = sound.Pitch;

        source.Play();

        yield return new WaitUntil(() => !source.isPlaying);
        
        Release(source);
    }

    IEnumerator PlayRoutine(SoundEffect sound) {
        yield return PlayRoutine(sound, GetSource(sound));
    }
    
    AudioSource GetSource(SoundEffect sound) {
        if (_inactiveSources.Count == 0) {
            CreateSource();
        }
        
        var source = _inactiveSources.Dequeue();
        _activeSources.Add(source, sound);
        SetVolume(source, sound);
        return source;
    }

    void Release(AudioSource source) {
        if (source == null || !_activeSources.ContainsKey(source)) {
            return;
        }
        
        source.Stop();
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