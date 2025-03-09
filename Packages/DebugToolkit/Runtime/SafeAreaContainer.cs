#nullable enable

using UnityEngine.UIElements;
#if UNITY_EDITOR
using Screen = UnityEngine.Device.Screen;
#else
using Screen = UnityEngine.Screen;
#endif

namespace DebugToolkit
{
    /// <summary>
    /// セーフエリアを考慮した<see cref="VisualElement"/>.
    /// </summary>
#if UNITY_2023_2_OR_NEWER
    [UxmlElement]
    public partial class SafeAreaContainer : VisualElement
    {
#else
    public class SafeAreaContainer : VisualElement
    {
        /// <summary>
        /// UIBuilderのLibraryに登録するためのUXML要素のファクトリクラス.
        /// </summary>
        public class SafeAreaContainerFactory : UxmlFactory<SafeAreaContainer, UxmlTraits>
        {
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeAreaContainer"/> class.
        /// </summary>
        public SafeAreaContainer()
        {
            style.flexGrow = 1;
            style.flexShrink = 1;

            RegisterCallback<GeometryChangedEvent>(static evt =>
            {
                var self = evt.target as SafeAreaContainer;
                if (self == null || self.panel.GetType().Name == "EditorPanel") return;
                var safeArea = Screen.safeArea;
                var leftTop = RuntimePanelUtils.ScreenToPanel(self.panel, new(safeArea.xMin, Screen.height - safeArea.yMax));
                var rightBottom = RuntimePanelUtils.ScreenToPanel(self.panel, new(Screen.width - safeArea.xMax, safeArea.yMin));

                self.style.marginLeft = leftTop.x;
                self.style.marginTop = leftTop.y;
                self.style.marginRight = rightBottom.x;
                self.style.marginBottom = rightBottom.y;
            });
        }
    }
}
