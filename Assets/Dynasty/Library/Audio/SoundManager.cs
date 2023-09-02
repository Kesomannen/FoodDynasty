using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace Dynasty.Library.Audio {

public class SoundManager : MonoBehaviour {
    public static float MasterVolume { get; set; }
    public static float MusicVolume { get; set; }
    public static float EffectsVolume { get; set; }
    
    ObjectPool<AudioSource> _pool;
    
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
            return obj;
        });
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