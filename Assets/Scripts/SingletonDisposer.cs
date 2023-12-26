using Dynasty.Library;
using UnityEngine;

public static class SingletonDisposer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void DisposeSingleton()
    {
        SoundManager.Dispose();
        TickManager.Dispose();
        CoroutineHelpers.Dispose();
        LeanTween.reset();
    }
}
