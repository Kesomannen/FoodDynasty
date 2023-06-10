#if !UNITY_EDITOR
using ExtEvents;
using UnityEngine;
using UnityEngine.Scripting;

public static class LoadConverterTypes
{
    // AfterAssembliesLoaded should be early enough that no one invokes ExtEvent. But if it's not early enough, we might try SubsystemRegistration, it runs even before that.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded), Preserve]
    public static void OnLoad()
    {
        AOTGeneratedType.OnLoad();
    }
}
#endif
