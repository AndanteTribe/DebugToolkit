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
        ///  is this window last operated?
        ///  </summary>
        public bool IsLastOperated { get; set; } = false;

        /// <summary>
        ///  has an Enable/Disable toggle button at parent window.
        ///  master-window does not use this.
        /// </summary>
        public Toggle VisibilityToggleButton { get; set; }
    }
}