using Dynasty.Library;
using UnityEngine;

public class SoundPlayer : MonoBehaviour {
    [SerializeField] SoundEffect _soundEffect;
    
    public void PlaySound() {
        _soundEffect.Play();
    }
}