#nullable enable

using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace DebugToolkit
{
    public static partial class DebugExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddProfileInfoLabel(this VisualElement visualElement) 
        {
            var label = new Label{ enableRichText = true };
            visualElement.Add(label);
            label.schedule.Execute(() =>
            {
                // Format Example:
                // <b>Performance</b>
                // CPU: 60fps (16.7ms)
                // GPU: 60fps (16.7ms)
                // Memory: 0.52GB
                var sb = new ValueStringBuilder(stackalloc char[128]);
                sb.Append(stackalloc char[]{ '<', 'b', '>', 'P', 'e', 'r', 'f', 'o', 'r', 'm', 'a', 'n', 'c', 'e', '<', '/', 'b', '>' });
                sb.AppendLine();

                var latest = ProfileUtils.GetLatestFrameTiming();
                sb.Append(stackalloc char[]{ 'C', 'P', 'U', ':', ' ' });
                sb.Append(1000 / latest.cpuFrameTime, "F0");
                sb.Append(stackalloc char[]{ 'f', 'p', 's', ' ', '(', ' ' });
                sb.Append(latest.cpuFrameTime, "F1");
                sb.Append(stackalloc char[]{ 'm', 's', ')' });
                sb.AppendLine();
                sb.Append(stackalloc char[]{ 'G', 'P', 'U', ':', ' ' });
                sb.Append(1000 / latest.gpuFrameTime, "F0");
                sb.Append(stackalloc char[]{ 'f', 'p', 's', ' ', '(', ' ' });
                sb.Append(latest.gpuFrameTime, "F1");
                sb.Append(stackalloc char[]{ 'm', 's', ')' });
                sb.AppendLine();

                var memory = ProfileUtils.GetTotalMemoryGB();
                sb.Append(stackalloc char[]{ 'M', 'e', 'm', 'o', 'r', 'y', ':', ' ' });
                sb.Append(memory, "F2");
                sb.Append(stackalloc char[]{ 'G', 'B' });
                sb.AppendLine();

                label.text = sb.ToString();
            }).Every(500);
        }
    }
}