#nullable enable

using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
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
        public static TabView AddTabView(this VisualElement visualElement)
        {
            var tabView = new TabView();
            visualElement.Add(tabView);
            return tabView;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Tab AddTab(this TabView tabView)
        {
            var tab = new Tab();
            tabView.Add(tab);
            return tab;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Tab AddTab(this TabView tabView, string label)
        {
            var tab = new Tab(label);
            tabView.Add(tab);
            return tab;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Tab AddTab(this TabView tabView, Background iconImage)
        {
            var tab = new Tab(iconImage);
            tabView.Add(tab);
            return tab;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Tab AddTab(this TabView tabView, string label, Background iconImage)
        {
            var tab = new Tab(label, iconImage);
            tabView.Add(tab);
            return tab;
        }
#endif
    }
}
