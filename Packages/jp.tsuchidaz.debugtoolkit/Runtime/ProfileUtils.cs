using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Profiling;

namespace DebugToolkit
{
    public static class ProfileUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetTotalMemoryGB() => Profiler.GetTotalReservedMemoryLong() / MathF.Pow(1024f, 3);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FrameTiming GetLatestFrameTiming()
        {
#if UNITY_EDITOR
            if (!UnityEditor.PlayerSettings.enableFrameTimingStats)
            {
                UnityEditor.PlayerSettings.enableFrameTimingStats = true;
                UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                
                static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
                {
                    if (state == UnityEditor.PlayModeStateChange.EnteredEditMode)
                    {
                        UnityEditor.PlayerSettings.enableFrameTimingStats = true;
                        UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                    }
                }
            }
#endif
            FrameTimingManager.CaptureFrameTimings();
            var timings = ArrayPool<FrameTiming>.Shared.Rent(1);
            var result = FrameTimingManager.GetLatestTimings(1, timings) == 0 ? default : timings[0];
            ArrayPool<FrameTiming>.Shared.Return(timings);
            return result;
        }
    }
}