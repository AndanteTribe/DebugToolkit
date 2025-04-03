#nullable enable

using System.Collections.Generic;
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
        /// Static collection that maintains references to all debug windows in the application.
        /// Used for global operations such as toggling visibility of all windows at once.
        /// </summary>
        protected internal static readonly List<VisualElement>  DebugWindowList = new();

        /// <summary>
        /// Custom <see cref="UnityEngine.UIElements.PanelSettings"/>.
        /// </summary>
        public PanelSettings? PanelSettings { get; set; }

        /// <summary>
        /// Custom <see cref="UnityEngine.UIElements.ThemeStyleSheet"/>.
        /// </summary>
        public ThemeStyleSheet? ThemeStyleSheet { get; set; }

        /// <summary>
        /// Reference to the main window that contains controls for managing all other debug windows.
        /// Provides functionality for toggling visibility and accessing the list of all debug windows.
        /// </summary>
        protected internal static VisualElement? MasterWindow { get; set; }

        /// <summary>
        /// Flag that controls the visibility of all windows.
        /// When true, all windows are displayed; when false, all windows are hidden.
        /// </summary>
        private static bool s_allWindowsVisible = true;

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
            if (PanelSettings == null)
            {
                PanelSettings = ExternalResources.LoadPanelSettings();
            }
            if (PanelSettings.themeStyleSheet == null)
            {
                if (ThemeStyleSheet == null)
                {
                    ThemeStyleSheet = ExternalResources.LoadThemeStyleSheet();
                }
                PanelSettings.themeStyleSheet = ThemeStyleSheet;
            }
            uiDocument.panelSettings = PanelSettings;

            var root = uiDocument.rootVisualElement;
            var safeAreaContainer = new SafeAreaContainer();
            safeAreaContainer.pickingMode = PickingMode.Ignore;
            root.Add(safeAreaContainer);

            if (MasterWindow == null)
            {
                var masterWindow = safeAreaContainer.AddWindow("Debug Toolkit", false);

                var windowList = new ScrollView();
                windowList.AddToClassList(DebugExtensions.DebugToolkitClassName + "__window-list");
                masterWindow.Add(windowList);
                var lable = new Label("Debug Window List");
                windowList.Add(lable);

                MasterWindow = masterWindow;

                var toggleAllButton = new Button();
                toggleAllButton.clicked += () => ToggleAllVisible();
                toggleAllButton.AddToClassList(DebugExtensions.DebugToolkitClassName + "__toggle-all-button");
                safeAreaContainer.Add(toggleAllButton);
            }

            return safeAreaContainer;
        }

        /// <summary>
        /// Toggles the visibility of all debug windows.
        /// Based on the current visibility state, shows or hides all windows.
        /// Also synchronizes the state of toggle buttons in the master window.
        /// </summary>
        private static void ToggleAllVisible()
        {
            s_allWindowsVisible = !s_allWindowsVisible;
            foreach (var debugWindow in DebugWindowList)
            {
                debugWindow.style.display = s_allWindowsVisible ? DisplayStyle.Flex : DisplayStyle.None;
            }

            if (MasterWindow != null)
            {
                var windowList = MasterWindow.Q<ScrollView>(className: DebugExtensions.DebugToolkitClassName + "__window-list");
                if (windowList != null)
                {
                    foreach (var listItem in windowList.Children())
                    {
                        var toggle = listItem.Q<Toggle>();
                        if (toggle != null)
                        {
                            toggle.SetValueWithoutNotify(s_allWindowsVisible);
                        }
                    }
                }
            }
        }
    }
}
