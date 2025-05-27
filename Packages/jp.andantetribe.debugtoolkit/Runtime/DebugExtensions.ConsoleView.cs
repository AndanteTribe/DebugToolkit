#nullable enable

using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace DebugToolkit
{
    public static partial class DebugExtensions
    {
        public static VisualElement AddConsoleView(this VisualElement root)
        {
            bool showLog = true, showWarning = true, showError = true, showStackTrace = false;
            var searchQuery = "";
            var listView = new ListView();
            List<(string message, string stackTrace, LogType type)> logs = new();

            Application.LogCallback logCallback = (msg, st, t) =>
            {
                logs.Add((msg, st, t));
                RefreshList();
            };

            Application.logMessageReceived += logCallback;

            var consoleView = new VisualElement();
            consoleView.AddToClassList(DebugConst.ClassName + "__console-view");

            consoleView.RegisterCallback<DetachFromPanelEvent>(evt =>
            {
                Application.logMessageReceived -= logCallback;
            });

            var toggleContainer = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            var logToggle = new Toggle("Log") { value = showLog };
            var warningToggle = new Toggle("Warning") { value = showWarning };
            var errorToggle = new Toggle("Error") { value = showError };
            var stackTraceToggle= new Toggle("StackTrace") { value = showStackTrace };

            void OnToggleChange(ChangeEvent<bool> _)
            {
                showLog = logToggle.value;
                showWarning = warningToggle.value;
                showError = errorToggle.value;
                showStackTrace = stackTraceToggle.value;
                RefreshList();
            }

            logToggle.RegisterValueChangedCallback(OnToggleChange);
            warningToggle.RegisterValueChangedCallback(OnToggleChange);
            errorToggle.RegisterValueChangedCallback(OnToggleChange);
            toggleContainer.Add(logToggle);
            toggleContainer.Add(warningToggle);
            toggleContainer.Add(errorToggle);
            consoleView.Add(toggleContainer);

            // Search Field
            var searchField = new TextField("Search");
            consoleView.Add(searchField);
            searchField.RegisterValueChangedCallback(evt =>
            {
                searchQuery = evt.newValue.ToLowerInvariant();
                RefreshList();
            });

            // Clear Button for clearing logs
            var clearBtn = new Button { text = "Clear" };
            clearBtn.clicked += () =>
            {
                logs.Clear();
                RefreshList();
            };
            consoleView.Add(clearBtn);

            // ListView
            listView = new ListView(logs, 20, () => new Label(), (ve, i) =>
                {
                    var label = ve as Label;
                    var currentItemsSource = listView.itemsSource as List<(string message, string stackTrace, LogType type)>;
                    if (currentItemsSource == null || i < 0 || i >= currentItemsSource.Count)
                    {
                        label!.text = string.Empty;
                        label!.style.backgroundColor = new StyleColor(Color.white);
                        return;
                    }
                    var entry = currentItemsSource[i];
                    switch (entry.type)
                    {
                        case LogType.Warning:
                            label!.style.backgroundColor = new StyleColor(new Color(1f, 1f, 0.6f));
                            break;
                        case LogType.Error:
                            label!.style.backgroundColor = new StyleColor(new Color(1f, 0.4f, 0.4f));
                            break;
                        default:
                            label!.style.backgroundColor = new StyleColor(Color.white);
                            break;
                    }

                    label.text = showStackTrace
                        ? $"[{entry.type}] {entry.message}\n{entry.stackTrace}"
                        : $"[{entry.type}] {entry.message}";
                })
                { style = { flexGrow = 1 } };

            consoleView.Add(listView);
            root.Add(consoleView);

            return consoleView;

            // local method
            void RefreshList()
            {
                var filtered = new List<(string message, string stackTrace, LogType type)>();
                foreach (var log in logs)
                {
                    if ((log.type == LogType.Log && !showLog) ||
                        (log.type == LogType.Warning && !showWarning) ||
                        (log.type == LogType.Error && !showError))
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(searchQuery) &&
                        !log.message.ToLowerInvariant().Contains(searchQuery))
                    {
                        continue;
                    }

                    filtered.Add(log);
                }

                listView.itemsSource = filtered;
                listView.Rebuild();
            }
        }
    }
}
