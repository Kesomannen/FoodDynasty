using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sound Effect")]
public class SoundEffect : ScriptableObject {
    [SerializeField] [Range(0, 1)] float _volume = 0.5f;
    [SerializeField] [Range(-3, 3)] float _pitch = 1;
    [SerializeField] AudioClip[] _clips;
    
    public float Volume => _volume;
    public float Pitch => _pitch;
    public IReadOnlyList<AudioClip> Clips => _clips;
    
    public void Play() {
        SoundManager.Instance.Play(this);
    }
}