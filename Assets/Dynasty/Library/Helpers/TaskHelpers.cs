using System.Collections;
using System.Threading.Tasks;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Library {

public static class TaskHelpers {
    public static Task Wait(float seconds) {
        return Task.Delay((int) (seconds * 1000));
    }
    
    public static IEnumerator AsCoroutine(this Task task) {
        while (!task.IsCompleted) {
            yield return null;
        }
    }
    
    public static TaskHandle<T> GetHandle<T>(this Task<T> task) {
        return new TaskHandle<T>(task);
    }
}

public class TaskHandle<T> : IEnumerator {
    public bool IsDone { get; private set; }
    public T Result { get; private set; }

    public TaskHandle(Task<T> task) {
        task.ContinueWith(t => {
            IsDone = true;
            Result = t.Result;
        });
    }
    
    public bool MoveNext() => !IsDone;
    public void Reset() { }
    public object Current => null;
}

}