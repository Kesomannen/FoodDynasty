using System;
using System.Collections;
using UnityEngine;

namespace Dynasty.Library {

public class CoroutineHandle : IEnumerator {
    public bool IsDone { get; private set; }
    
    public event Action OnComplete;

    public CoroutineHandle(MonoBehaviour behaviour, IEnumerator routine) {
        behaviour.StartCoroutine(PlayRoutine(routine));
    }
    
    IEnumerator PlayRoutine(IEnumerator routine) {
        yield return routine;
        IsDone = true;
        OnComplete?.Invoke();
    }
    
    public bool MoveNext() => !IsDone;
    public void Reset() { }
    public object Current => null;
}

}