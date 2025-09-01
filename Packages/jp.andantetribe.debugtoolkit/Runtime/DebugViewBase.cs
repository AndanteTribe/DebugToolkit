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
        /// Custom <see cref="UnityEngine.UIElements.PanelSettings"/>.
        /// </summary>
        public PanelSettings? PanelSettings { get; set; }

        /// <summary>
        /// Custom <see cref="UnityEngine.UIElements.ThemeStyleSheet"/>.
        /// </summary>
        public ThemeStyleSheet? ThemeStyleSheet { get; set; }

        /// <summary>
        /// EntryPoint.
        /// </summary>
        public void Start() => CreateViewGUI();

        /// <summary>
        /// The main window
        /// </summary>
        public VisualElement? MasterWindow { get; private set; }

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
            // Store instance reference for use by extension methods
            safeAreaContainer.userData = this;
            safeAreaContainer.AddToClassList(DebugConst.SafeAreaContainerClassName);
            root.Add(safeAreaContainer);

            MasterWindow = safeAreaContainer.AddWindow("Debug Toolkit");

            var label = new Label("Debug Window List");
            MasterWindow.Add(label);

            // 全表示非表示ボタンは1つだけ作成
            if (root.Query<Button>(className: DebugConst.ClassName + "__toggle-all-button").ToList().Count == 0)
            {
                var toggleAllButton = new Button();
                toggleAllButton.RegisterCallback<ClickEvent>((_)
                    => ToggleAllVisible());
                toggleAllButton.AddToClassList(DebugConst.ClassName + "__toggle-all-button");
                safeAreaContainer.Add(toggleAllButton);
            }

            return MasterWindow;
        }

        /// <summary>
        /// Toggles the visibility of all debug windows.
        /// Based on the current visibility state, shows or hides all windows.
        /// Also synchronizes the state of toggle buttons in the master window.
        /// </summary>
        private void ToggleAllVisible()
        {
            if (MasterWindow != null)
            {
                var debugWindows = MasterWindow.GetAllDebugWindows();
                if (debugWindows.Count == 0)
                {
                    return;
                }

                var isAnyVisibleEnable = false;

                foreach (var window in debugWindows)
                {
                    if (window.style.display == DisplayStyle.Flex)
                    {
                        isAnyVisibleEnable = true;
                        break;
                    }
                }

                foreach (var window in debugWindows)
                {
                    if (isAnyVisibleEnable)
                    {
                        SetVisibility(window, false);
                    }else if (ShouldBeVisibleByDefault(window))
                    {
                        SetVisibility(window, true);
                    }
                }
            }
        }

        /// <summary>
        /// Should the window be visible by default?
        /// The last operated window and windows with the master window class name are visible by default.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        private static bool ShouldBeVisibleByDefault(DebugWindow window) =>
                window.IsLastOperated ||
                window.ClassListContains(DebugConst.MasterWindowClassName) ||
                window.GetDebugWindowParent().ClassListContains(DebugConst.MasterWindowClassName);

        /// <summary>
        /// Sets the visibility of the specified debug window and updates the state of its toggle button.
        /// </summary>
        /// <param name="window">The debug window to update.</param>
        /// <param name="visible">True to show the window, false to hide it.</param>
        private static void SetVisibility(DebugWindow window, bool visible)
        {
            window.VisibilityToggleButton.value = visible;
            window.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
