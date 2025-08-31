using UnityEngine.UIElements;

namespace DebugToolkit
{
#if UNITY_2023_2_OR_NEWER
    /// <summary>
    ///  <see cref="VisualElement"/> for debug window
    /// </summary>
    [UxmlElement]
    internal sealed partial class DebugWindow : VisualElement
    {
#else
    internal sealed class DebugWindow : VisualElement
    {
        /// <summary>
        /// UIBuilderのLibraryに登録するためのUXML要素のファクトリクラス.
        /// </summary>
        public class SafeAreaContainerFactory : UxmlFactory<SafeAreaContainer, UxmlTraits>
        {
        }
#endif
        /// <summary>
        /// this is the last operated window.
        ///  </summary>
        public bool IsLastOperated { get; set; } = false;

        /// <summary>
        ///   has a Enable/Disable toggle button at parent window.
        ///  master-window does not use this.
        /// </summary>
        public Toggle VisibilityToggleButton { get; private set; } = new Toggle();
    }
}