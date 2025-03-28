#nullable enable

using UnityEngine;
using UnityEngine.UIElements;

namespace DebugToolkit
{
    /// <summary>
    /// Base class for the implementation of the debugging menu on the real machine.
    /// </summary>
    public abstract class DebugViewerBase
    {
        /// <summary>
        /// Custom <see cref="PanelSettings"/>.
        /// </summary>
        public PanelSettings? panelSettings { get; set; }

        /// <summary>
        /// Custom <see cref="ThemeStyleSheet"/>.
        /// </summary>
        public ThemeStyleSheet? themeStyleSheet { get; set; }

        /// <summary>
        /// EntryPoint.
        /// </summary>
        public void Start() => CreateViewGUI();

        /// <summary>
        /// Implement this method to make a custom UIElements viewer.
        /// </summary>
        /// <returns>Root <see cref="VisualElement"/>.</returns>
        protected virtual VisualElement CreateViewGUI()
        {
            var obj = new GameObject(nameof(DebugToolkit));
            Object.DontDestroyOnLoad(obj);
            var uiDocument = obj.AddComponent<UIDocument>();
            if (panelSettings == null)
            {
                panelSettings = ExternalResources.LoadPanelSettings();
            }
            if (panelSettings.themeStyleSheet == null)
            {
                if (themeStyleSheet == null)
                {
                    themeStyleSheet = ExternalResources.LoadThemeStyleSheet();
                }
                panelSettings.themeStyleSheet = themeStyleSheet;
            }
            uiDocument.panelSettings = panelSettings;

            var root = uiDocument.rootVisualElement;
            var safeAreaContainer = new SafeAreaContainer();
            root.Add(safeAreaContainer);

            var window = new VisualElement();
            window.AddToClassList("debug-toolkit-master");
            safeAreaContainer.Add(window);

            var manipulator = new DragManipulator(window);
            var dragArea = new VisualElement(){ name = "drag-area" };
            dragArea.AddToClassList("unity-foldout__drag-area");
            dragArea.AddManipulator(manipulator);

            var foldout = new Foldout { text = "DebugMenu", value = true, name = "DebugMenuHandler" };
            foldout.Q<Toggle>().Add(dragArea);
            window.Add(foldout);

            return foldout;
        }
    }
}
