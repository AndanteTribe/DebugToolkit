using System.Collections.Generic;
using UnityEngine.UIElements;

namespace DebugToolkit
{
    public static class DebugStatic
    {
        /// <summary>
        /// Reference to the main window that contains controls for managing all other debug windows.
        /// Provides functionality for toggling visibility and accessing the list of all debug windows.
        /// </summary>
        internal static VisualElement Master { get; set; }

        /// <summary>
        /// Collection that maintains references to all debug windows in this instance.
        /// Used for operations such as toggling visibility of all windows at once.
        /// </summary>
        internal static List<VisualElement> WindowList { get; private set; } = new List<VisualElement>();
    }
}