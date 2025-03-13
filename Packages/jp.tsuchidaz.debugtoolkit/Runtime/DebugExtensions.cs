#nullable enable

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace DebugToolkit
{
    public static partial class DebugExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddProfileInfoLabel(this VisualElement visualElement) 
        {
#if UNITY_EDITOR
            UnityEditor.PlayerSettings.enableFrameTimingStats = true;
#endif
            var label = new Label{ enableRichText = true };
            visualElement.Add(label);
            label.schedule.Execute(() =>
            {
                // Format Example:
                // <b>Performance</b>
                // CPU: 60fps (16.7ms)
                // GPU: 60fps (16.7ms)
                // Memory: 0.52GB
                var latest = GetLatestFrameTiming();
                var memory = GetTotalMemoryGB();
                label.text = string.Create(128, (latest, memory), static (span, args) =>
                {
                    var (latest, memory) = args;
                    var line = Environment.NewLine.AsSpan();
                    stackalloc char[]{ '<', 'b', '>', 'P', 'e', 'r', 'f', 'o', 'r', 'm', 'a', 'n', 'c', 'e', '<', '/', 'b', '>' }.CopyTo(span);
                    span = span.Slice(18);
                    line.CopyTo(span);
                    span = span.Slice(line.Length);
                    stackalloc char[]{ 'C', 'P', 'U', ':', ' ' }.CopyTo(span);
                    span = span.Slice(5);
                    (1000 / latest.cpuFrameTime).TryFormat(span, out var written, "F0");
                    span = span.Slice(written);
                    stackalloc char[]{ 'f', 'p', 's', ' ', '(', ' ' }.CopyTo(span);
                    span = span.Slice(6);
                    latest.cpuFrameTime.TryFormat(span, out written, "F1");
                    span = span.Slice(written);
                    stackalloc char[]{ 'm', 's', ')' }.CopyTo(span);
                    span = span.Slice(3);
                    line.CopyTo(span);
                    span = span.Slice(line.Length);
                    stackalloc char[]{ 'G', 'P', 'U', ':', ' ' }.CopyTo(span);
                    span = span.Slice(5);
                    (1000 / latest.gpuFrameTime).TryFormat(span, out written, "F0");
                    span = span.Slice(written);
                    stackalloc char[]{ 'f', 'p', 's', ' ', '(', ' ' }.CopyTo(span);
                    span = span.Slice(6);
                    latest.gpuFrameTime.TryFormat(span, out written, "F1");
                    span = span.Slice(written);
                    stackalloc char[]{ 'm', 's', ')' }.CopyTo(span);
                    span = span.Slice(3);
                    line.CopyTo(span);
                    span = span.Slice(line.Length);
                    stackalloc char[]{ 'M', 'e', 'm', 'o', 'r', 'y', ':', ' ' }.CopyTo(span);
                    span = span.Slice(8);
                    memory.TryFormat(span, out written, "F2");
                    span = span.Slice(written);
                    stackalloc char[]{ 'G', 'B' }.CopyTo(span);
                    span = span.Slice(2);
                    line.CopyTo(span);
                    span = span.Slice(line.Length);
                    for (int i = 0; i < span.Length; i++)
                    {
                        span[i] = ' ';
                    }
                });
            }).Every(500);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetTotalMemoryGB()
            => UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / Mathf.Pow(1024f, 3);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static FrameTiming GetLatestFrameTiming()
        {
            FrameTimingManager.CaptureFrameTimings();
            var timings = ArrayPool<FrameTiming>.Shared.Rent(1);
            var result = FrameTimingManager.GetLatestTimings(1, timings) == 0 ? default : timings[0];
            ArrayPool<FrameTiming>.Shared.Return(timings);
            return result;
        }
    }
}