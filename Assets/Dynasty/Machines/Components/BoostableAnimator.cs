using Dynasty.Library.Classes;
using UnityEngine;

namespace Dynasty.Machines {

[RequireComponent(typeof(Animator))]
public class BoostableAnimator : MonoBehaviour, IBoostable {
    public Modifier Modifier {
        get => _modifier;
        set {
            _modifier = value;
            _animator.speed = (float) _modifier.Delta;
        }
    }

    Animator _animator;
    Modifier _modifier;

    void Awake() {
        _animator = GetComponent<Animator>();
        Modifier = new Modifier(@base: 1);
    }
}

}