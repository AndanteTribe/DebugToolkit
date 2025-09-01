using UnityEngine.UIElements;

namespace DebugToolkit
{
    /// <summary>
    ///  <see cref="VisualElement"/> for debug window
    /// </summary>
#if UNITY_2023_2_OR_NEWER
    [UxmlElement]
    internal sealed partial class DebugWindow : VisualElement
    {
#else
    internal sealed class DebugWindow : VisualElement
    {
        /// <summary>
        /// UIBuilderのLibraryに登録するためのUXML要素のファクトリクラス.
        /// </summary>
        public class DebugWindowFactory : UxmlFactory<DebugWindow, UxmlTraits>
        {
        }
#endif
        /// <summary>
        /// Indicates whether this debug window was the most recently interacted with window.
        /// Used to determine focus and input priority among multiple debug windows.
        /// </summary>
        public bool IsLastOperated { get; set; } = false;

        /// <summary>
        /// Reference to the toggle button that controls this window's visibility state.
        /// This toggle is typically located in the parent window's UI.
        /// Note: The master window does not utilize this toggle button.
        /// </summary>
        public Toggle VisibilityToggleButton { get; set; }
    }
}