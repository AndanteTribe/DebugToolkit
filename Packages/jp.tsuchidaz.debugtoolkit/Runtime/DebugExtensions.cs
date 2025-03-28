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
                sb.Append(1000 / latest.cpuFrameTime, stackalloc char[]{ 'F', '0' });
                sb.Append(stackalloc char[]{ 'f', 'p', 's', ' ', '(', ' ' });
                sb.Append(latest.cpuFrameTime, stackalloc char[]{ 'F', '1' });
                sb.Append(stackalloc char[]{ 'm', 's', ')' });
                sb.AppendLine();
                sb.Append(stackalloc char[]{ 'G', 'P', 'U', ':', ' ' });
                sb.Append(1000 / latest.gpuFrameTime, stackalloc char[]{ 'F', '0' });
                sb.Append(stackalloc char[]{ 'f', 'p', 's', ' ', '(', ' ' });
                sb.Append(latest.gpuFrameTime, stackalloc char[]{ 'F', '1' });
                sb.Append(stackalloc char[]{ 'm', 's', ')' });
                sb.AppendLine();

                var memory = ProfileUtils.GetTotalMemoryGB();
                sb.Append(stackalloc char[]{ 'M', 'e', 'm', 'o', 'r', 'y', ':', ' ' });
                sb.Append(memory, stackalloc char[]{ 'F', '2' });
                sb.Append(stackalloc char[]{ 'G', 'B' });
                sb.AppendLine();

                label.text = sb.ToString();
            }).Every(500);
        }

#if UNITY_2023_2_OR_NEWER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScrollView AddTab(this TabView tabView, string label = "")
        {
            var tab = string.IsNullOrEmpty(label) ? new Tab() : new Tab(label);
            tabView.Add(tab);
            var scrollView = new ScrollView();
            tab.Add(scrollView);
            return scrollView;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (TabView,  ScrollView) AddTab(this VisualElement root, string label = "")
        {
            var tabView = new TabView();
            root.Add(tabView);
            return (tabView, tabView.AddTab(label));
        }
#endif
    }
}
