using System;
using System.Collections;
using System.Collections.Generic;
using Dynasty.Library.Classes;
using UnityEngine;

namespace Dynasty.Library.Audio {

[CreateAssetMenu(menuName = "Sound Effect")]
public class SoundEffect : ScriptableObject {
    [SerializeField] SoundType _type;
    [SerializeField] [Range(0, 1)] float _volume = 0.5f;
    [SerializeField] [Range(-3, 3)] float _pitch = 1;
    [SerializeField] AudioClip[] _clips;
    
    public SoundType Type => _type;
    public float Volume => _volume;
    public float Pitch => _pitch;
    public IReadOnlyList<AudioClip> Clips => _clips;
    
    public void Play() {
        SoundManager.Singleton.Play(this);
    }
    
    public CoroutineHandle Play(out Action cancel) {
        return SoundManager.Singleton.Play(this, out cancel);
    }
}

}