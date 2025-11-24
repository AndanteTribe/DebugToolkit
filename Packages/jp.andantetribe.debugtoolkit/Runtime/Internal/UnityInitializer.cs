#nullable enable
using UnityEngine;

[assembly: UnityEngine.Scripting.AlwaysLinkAssembly]

namespace DebugToolkit
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    internal sealed class UnityInitializer
    {
        static UnityInitializer()
        {
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void OnRuntimeInitializeOnLoad()
        {
            // do not support logs for non-play sessions.
            ConsoleView.Initialize();
        }
    }
}