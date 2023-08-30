using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public class SoundManager : MonoBehaviour {
    ObjectPool<AudioSource> _pool;
    
    static SoundManager _instance;
    
    public static SoundManager Instance {
        get {
            if (_instance != null) return _instance;
            
            _instance = new GameObject("SoundManager").AddComponent<SoundManager>();
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

    public async void Play(SoundEffect sound) {
        var source = _pool.Get();
        
        source.clip = sound.Clips[Random.Range(0, sound.Clips.Count)];
        source.volume = sound.Volume;
        source.pitch = sound.Pitch;
        
        source.Play();

        while (source.isPlaying) {
            await Task.Yield();
        }
        
        _pool.Release(source);
    }
}