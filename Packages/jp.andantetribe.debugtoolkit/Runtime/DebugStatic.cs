using System.Collections.Generic;
using UnityEngine.UIElements;

namespace DebugToolkit
{
    internal static class DebugStatic
    {
        internal static bool s_allWindowsVisible = true;
        internal static List<VisualElement> WindowList { get; } = new List<VisualElement>();
    }
}