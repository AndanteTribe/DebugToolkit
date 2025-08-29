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
        /// Custom <see cref="UnityEngine.UIElements.PanelSettings"/>.
        /// </summary>
        public PanelSettings? PanelSettings { get; set; }

        /// <summary>
        /// Custom <see cref="UnityEngine.UIElements.ThemeStyleSheet"/>.
        /// </summary>
        public ThemeStyleSheet? ThemeStyleSheet { get; set; }

        /// <summary>
        /// Flag that controls the visibility of all windows.
        /// When true, all windows are displayed; when false, all windows are hidden.
        /// </summary>
        private bool _allWindowsVisible = true;

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
            // Store instance reference for use by extension methods
            safeAreaContainer.userData = this;
            root.Add(safeAreaContainer);

            if (DebugStatic.Master == null)
            {
                var masterWindow = safeAreaContainer.AddWindow("Debug Toolkit");

                var windowList = new ScrollView();
                windowList.AddToClassList(DebugConst.WindowListClassName);
                masterWindow.Add(windowList);
                var label = new Label("Debug Window List");
                windowList.Add(label);
                DebugStatic.Master = masterWindow;

                var toggleAllButton = new Button() {text = "A"};
                toggleAllButton.RegisterCallback<ClickEvent, DebugViewerBase>((evt, t) => t.ToggleAllVisible(), this);
                toggleAllButton.AddToClassList(DebugConst.ClassName + "__toggle-all-button");
                safeAreaContainer.Add(toggleAllButton);

                var masterButton = new Button() {text = "M"};
                masterButton.RegisterCallback<ClickEvent>( (evt) =>
                {
                    var masterWindowRoot = DebugStatic.Master.parent;
                    if (masterWindowRoot.style.display != DisplayStyle.None)
                    {
                        masterWindowRoot.userData = masterWindowRoot.style.display;
                        masterWindowRoot.style.display = DisplayStyle.None;
                        return;
                    }
                    else
                    {
                        if (masterWindowRoot.userData is StyleEnum<DisplayStyle> previous)
                        {
                            masterWindowRoot.style.display = previous;
                        }
                        masterWindowRoot.style.display = DisplayStyle.Flex;
                    }
                });
                masterButton.AddToClassList(DebugConst.ClassName + "__master-button");
                safeAreaContainer.Add(masterButton);
            }

            return safeAreaContainer;
        }

        /// <summary>
        /// Toggles the visibility of all debug windows.
        /// Based on the current visibility state, shows or hides all windows.
        /// Also synchronizes the state of toggle buttons in the master window.
        /// </summary>
        private void ToggleAllVisible()
        {
            _allWindowsVisible = !_allWindowsVisible;
            foreach (var window in DebugStatic.WindowList)
            {
                if (_allWindowsVisible  && window.userData is StyleEnum<DisplayStyle> previous)
                {
                    window.style.display = previous;
                }
                else
                {
                    window.userData = window.style.display;
                    window.style.display = DisplayStyle.None;
                }
            }
        }
    }
}
