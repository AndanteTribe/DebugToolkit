#nullable enable

using System;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using UnityEngine;

namespace DebugToolkit
{
    public static class DebugExtensions
    {
        private static readonly Vector2 s_windowOffsetStep = new Vector2(50f, 50f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddProfileInfoLabel(this VisualElement visualElement)
            => AddProfileInfoLabel(visualElement, TimeSpan.FromMilliseconds(500));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddProfileInfoLabel(this VisualElement visualElement, in TimeSpan interval)
        {
            var label = new Label{ enableRichText = true };
            visualElement.Add(label);
            label.schedule.Execute(() =>
            {
                // Format Example:
                // <b>Performance</b>
                // CPU: 60fps (16.7ms)
                // GPU: 60fps (16.7ms)
                // Memory: 0.52GB
                var sb = new ValueStringBuilder(stackalloc char[128]);
                sb.Append(stackalloc char[]{ '<', 'b', '>', 'P', 'e', 'r', 'f', 'o', 'r', 'm', 'a', 'n', 'c', 'e', '<', '/', 'b', '>' });
                sb.AppendLine();

                var latest = ProfileUtils.GetLatestFrameTiming();
                sb.Append(stackalloc char[]{ 'C', 'P', 'U', ':', ' ' });
                sb.Append(1000 / latest.cpuFrameTime, stackalloc char[]{ 'F', '0' });
                sb.Append(stackalloc char[]{ 'f', 'p', 's', ' ', '(', ' ' });
                sb.Append(latest.cpuFrameTime, stackalloc char[]{ 'F', '1' });
                sb.Append(stackalloc char[]{ 'm', 's', ')' });
                sb.AppendLine();
                sb.Append(stackalloc char[]{ 'G', 'P', 'U', ':', ' ' });
                sb.Append(1000 / latest.gpuFrameTime, stackalloc char[]{ 'F', '0' });
                sb.Append(stackalloc char[]{ 'f', 'p', 's', ' ', '(', ' ' });
                sb.Append(latest.gpuFrameTime, stackalloc char[]{ 'F', '1' });
                sb.Append(stackalloc char[]{ 'm', 's', ')' });
                sb.AppendLine();

                var memory = ProfileUtils.GetTotalMemoryGB();
                sb.Append(stackalloc char[]{ 'M', 'e', 'm', 'o', 'r', 'y', ':', ' ' });
                sb.Append(memory, stackalloc char[]{ 'F', '2' });
                sb.Append(stackalloc char[]{ 'G', 'B' });
                sb.AppendLine();

                label.text = sb.ToString();
            }).Every((int)interval.TotalMilliseconds);
        }

        /// <summary>
        /// Adds a new window to the specified root element.
        /// Creates a window with a header and content area, registers it for drag functionality,
        /// and adds it to the debug window list. If a master window exists, also adds this window to the window list.
        /// </summary>
        /// <param name="root">The root element to which the window will be added</param>
        /// <param name="windowName">The name/title of the window</param>
        /// <returns>The content area of the created window</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VisualElement AddWindow(this VisualElement root, string windowName = "")
        {
            var window = new VisualElement();
            root.Add(window);

            window.AddToClassList(DebugConst.DebugToolkitClassName + "__master");

            var isMasterWindow = DebugViewerBase.MasterWindow == null;
            window.AddWindowHeader(windowName, isMasterWindow);

            var windowContent = new VisualElement();
            windowContent.AddToClassList(DebugConst.DebugToolkitClassName + "__window-content");
            window.Add(windowContent);

            window.RegisterCallback<PointerDownEvent>(_ => window.BringToFront(), TrickleDown.TrickleDown);

            DebugViewerBase.DebugWindowList.Add(window);

            window.style.display = DisplayStyle.Flex;

            var windowNum = 1;
            if (!isMasterWindow)
            {
                AddWindowListItem(window, windowName);
                windowNum += DebugViewerBase.MasterWindow.Q<ScrollView>(className: DebugConst.DebugToolkitClassName + "__window-list").childCount;
            }

            window.style.position = Position.Absolute;
            window.style.left = s_windowOffsetStep.x * windowNum;
            window.style.top = s_windowOffsetStep.y * windowNum;

            return windowContent;
        }

        /// <summary>
        /// Adds a window list item to the master window.
        /// Includes a toggle for controlling visibility and a label with the window name.
        /// </summary>
        /// <param name="window">The window element to add to the list</param>
        /// <param name="windowName">The name of the window</param>
        private static void AddWindowListItem(VisualElement window, string windowName)
        {
            var windowList = DebugViewerBase.MasterWindow.Q<ScrollView>(className: DebugConst.DebugToolkitClassName + "__window-list");
            if (windowList == null) return;

            var listItem = new VisualElement();
            listItem.style.flexDirection = FlexDirection.Row;
            listItem.style.marginBottom = 5;

            var button = new Button()
            {
                text = windowName,
            };

            button.clicked += () =>
            {
                window.style.display = (window.style.display == DisplayStyle.Flex) ? DisplayStyle.None : DisplayStyle.Flex;
                UpdateWindowState(window, button);
                window.BringToFront();
            };

            window.style.display = DisplayStyle.None;
            listItem.Add(button);
            windowList.Add(listItem);
            UpdateWindowState(window, button);
        }

        /// <summary>
        /// Updates the window state.
        /// </summary>
        /// <param name="window">The window element.</param>
        /// <param name="button">The button element.</param>
        private static void UpdateWindowState(VisualElement window, Button button)
        {
            if (window.style.display == DisplayStyle.Flex)
            {
                button.style.backgroundColor = new StyleColor(new Color(0.4f, 0.8f, 0.4f)); // highlight green color
            }
            else
            {
                button.style.backgroundColor = new StyleColor(new Color(0.6f, 0.2f, 0.2f)); // dark red color
            }
        }

        /// <summary>
        /// Adds a header to a window.
        /// The header includes a window name, a draggable area, and optionally a delete button.
        /// </summary>
        /// <param name="root">The parent element to which the header will be added</param>
        /// <param name="windowName">The name of the window</param>
        ///  <param name="isMasterWindow">Indicates if this is the master window</param>
        /// <returns>The created header element</returns>
        public static VisualElement AddWindowHeader(this VisualElement root, string windowName = "", bool isMasterWindow = false)
        {
            var windowHeader = new VisualElement();
            windowHeader.AddToClassList(DebugConst.DebugToolkitClassName +"__window-header");

            var manipulator = new DragManipulator(root);
            var dragArea = new VisualElement(){ name = "drag-area" };
            dragArea.AddToClassList(DebugConst.DebugToolkitClassName + "__drag-area");
            dragArea.AddManipulator(manipulator);
            windowHeader.Add(dragArea);

            var windowLabel = new Label(){ text = windowName };
            windowLabel.AddToClassList(DebugConst.DebugToolkitClassName + "__window-label");
            dragArea.Add(windowLabel);

            if (isMasterWindow)
            {
                var minimizeButton = new Button() { text = "-" };
                minimizeButton.AddToClassList(DebugConst.DebugToolkitClassName + "__minimize-button");

                var isMinimized = false;

                minimizeButton.clicked += () =>
                {
                    isMinimized = !isMinimized;
                    var windowContent = root.Q<VisualElement>(className: DebugConst.DebugToolkitClassName + "__window-content");
                    if (windowContent != null)
                    {
                        windowContent.style.display = isMinimized ? DisplayStyle.None : DisplayStyle.Flex;
                    }
                    minimizeButton.text = isMinimized ? "^" : "-";
                };

                windowHeader.Add(minimizeButton);
            }
            else
            {
                var deleteButton = new Button() { text = "X" };
                deleteButton.AddToClassList(DebugConst.DebugToolkitClassName + "__delete-button");
                deleteButton.clicked += () =>
                {
                    root.style.display = DisplayStyle.None;

                    var windowList = DebugViewerBase.MasterWindow.Q<ScrollView>(
                        className: DebugConst.DebugToolkitClassName + "__window-list");

                    if (windowList != null)
                    {
                        foreach (var windowItem in windowList.Children())
                        {
                            var button = windowItem.Q<Button>();
                            if (button != null && button.text == windowName)
                            {
                                UpdateWindowState(root, button);
                            }
                        }
                    }
                };
                windowHeader.Add(deleteButton);
            }
            root.Add(windowHeader);
            return windowHeader;
        }

#if UNITY_2023_2_OR_NEWER
        /// <summary>
        /// Adds a new tab to a TabView.
        /// </summary>
        /// <param name="tabView">The TabView to which the tab will be added</param>
        /// <param name="label">The label for the tab</param>
        /// <returns>A ScrollView created inside the tab</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScrollView AddTab(this TabView tabView, string label = "")
        {
            var tab = string.IsNullOrEmpty(label) ? new Tab() : new Tab(label);
            tabView.Add(tab);
            var scrollView = new ScrollView();
            tab.Add(scrollView);
            return scrollView;
        }

        /// <summary>
        /// Adds a TabView and a tab to a parent element.
        /// </summary>
        /// <param name="root">The parent element to which the TabView will be added</param>
        /// <param name="label">The label for the tab</param>
        /// <returns>A tuple containing the added TabView and ScrollView</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (TabView,  ScrollView) AddTab(this VisualElement root, string label = "")
        {
            var tabView = new TabView();
            root.Add(tabView);
            return (tabView, tabView.AddTab(label));
        }
#endif
    }
}
