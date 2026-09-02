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
        /// Key that toggles the visibility of all debug windows, exactly like the toggle-all button
        /// at the bottom left of the screen. It also brings back the fixed debug menu (the master window).
        /// <see cref="KeyCode.None"/> (the default) disables the shortcut.
        /// The key is read every time it is pressed, so it can be changed at any time.
        /// </summary>
        /// <remarks>
        /// The key is picked up by the panel DebugToolkit renders into. If another UI Toolkit panel
        /// holds the keyboard focus, the shortcut does not fire there; call <see cref="ToggleAllVisible"/>
        /// from your own input handling in that case.
        /// </remarks>
        public KeyCode ToggleAllVisibleKey { get; set; } = KeyCode.None;

        /// <summary>
        /// Modifier keys that have to be held down together with <see cref="ToggleAllVisibleKey"/>.
        /// </summary>
        public EventModifiers ToggleAllVisibleKeyModifiers { get; set; } = EventModifiers.None;

        /// <summary>
        /// True while <see cref="ToggleAllVisibleKey"/> is held down, so that the auto-repeat of a
        /// held key does not toggle the windows over and over.
        /// </summary>
        private bool _isToggleAllVisibleKeyHeld;

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

            // Keyboard shortcut. Registered on the panel root with TrickleDown so that it is caught
            // wherever the focus is, including while a debug text field is being edited.
            root.RegisterCallback<KeyDownEvent>(OnToggleAllVisibleKeyDown, TrickleDown.TrickleDown);
            root.RegisterCallback<KeyUpEvent>(OnToggleAllVisibleKeyUp, TrickleDown.TrickleDown);

            var safeAreaContainer = new SafeAreaContainer();
            safeAreaContainer.pickingMode = PickingMode.Ignore;
            // Store instance reference for use by extension methods
            safeAreaContainer.userData = this;
            safeAreaContainer.AddToClassList(DebugConst.SafeAreaContainerClassName);
            root.Add(safeAreaContainer);

            MasterWindow = safeAreaContainer.AddWindow("Debug Toolkit");

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
        /// Handles <see cref="ToggleAllVisibleKey"/> being pressed.
        /// </summary>
        /// <param name="evt">The key down event.</param>
        private void OnToggleAllVisibleKeyDown(KeyDownEvent evt)
        {
            if (ToggleAllVisibleKey == KeyCode.None ||
                evt.keyCode != ToggleAllVisibleKey ||
                evt.modifiers != ToggleAllVisibleKeyModifiers)
            {
                return;
            }

            evt.StopPropagation();

            if (_isToggleAllVisibleKeyHeld)
            {
                return;
            }
            _isToggleAllVisibleKeyHeld = true;
            ToggleAllVisible();
        }

        /// <summary>
        /// Handles <see cref="ToggleAllVisibleKey"/> being released.
        /// </summary>
        /// <param name="evt">The key up event.</param>
        private void OnToggleAllVisibleKeyUp(KeyUpEvent evt)
        {
            if (evt.keyCode == ToggleAllVisibleKey)
            {
                _isToggleAllVisibleKeyHeld = false;
            }
        }

        /// <summary>
        /// Toggles the visibility of all debug windows.
        /// Based on the current visibility state, shows or hides all windows.
        /// Also synchronizes the state of toggle buttons in the master window.
        /// </summary>
        public void ToggleAllVisible()
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
                    }
                    else if (ShouldBeVisibleByDefault(window))
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
            if (window.VisibilityToggleButton != null)
                window.VisibilityToggleButton.value = visible;
            window.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}