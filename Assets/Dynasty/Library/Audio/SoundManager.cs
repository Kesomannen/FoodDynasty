using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Dynasty.Library.Audio {

public class SoundManager : MonoBehaviour {
    public static float MasterVolume { get; set; }
    public static float MusicVolume { get; set; }
    public static float EffectsVolume { get; set; }
    
    ObjectPool<AudioSource> _pool;
    List<AudioSource> _sources = new();
    
    static SoundManager _instance;
    
    public static SoundManager Instance {
        get {
            if (_instance != null) return _instance;
            
            _instance = new GameObject("Sound Manager").AddComponent<SoundManager>();
            DontDestroyOnLoad(_instance);

            return _instance;
        }
    }
    
    void Awake() {
        _pool = new ObjectPool<AudioSource>(() => {
            var obj = new GameObject("AudioSource").AddComponent<AudioSource>();
            DontDestroyOnLoad(obj);
            _sources.Add(obj);
            return obj;
        });
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (mode != LoadSceneMode.Single) return;
        foreach (var audioSource in _sources) {
            audioSource.Stop();
        }
    }

    public void Play(SoundEffect sound) {
        StartCoroutine(PlayRoutine(sound));
    }
    
    public IEnumerator PlayRoutine(SoundEffect sound) {
        var source = _pool.Get();
        
        source.clip = sound.Clips[Random.Range(0, sound.Clips.Count)];
        source.pitch = sound.Pitch;
        
        var volumeModifier = sound.Type == SoundType.Music ? MusicVolume : EffectsVolume;
        source.volume = sound.Volume * MasterVolume * volumeModifier;
        
        source.Play();

        yield return new WaitUntil(() => !source.isPlaying);
        
        _pool.Release(source);
    }
}

public enum SoundType {
    Effect,
    Music,
}

}