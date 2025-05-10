#nullable enable

using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace DebugToolkit
{
    public static partial class DebugExtensions
    {
        private static bool s_initialized;
        private static readonly List<(string message, string stackTrace, LogType type)> s_logs = new();
        private static ListView? s_listView;
        private static bool s_showLog = true;
        private static bool s_showWarning = true;
        private static bool s_showError = true;
        private static string s_searchQuery = string.Empty;

        public static VisualElement AddConsoleView(this VisualElement root, bool showStackTrace = false)
        {
            if (!s_initialized)
            {
                Application.logMessageReceived += (msg, st, t) =>
                {
                    s_logs.Add((msg, st, t));
                    RefreshList();
                };
                s_initialized = true;
            }

            var container = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    borderLeftWidth = 2,
                    borderRightWidth = 2,
                    borderTopWidth = 2,
                    borderBottomWidth = 2,
                    borderBottomColor = Color.gray,
                    borderTopColor = Color.gray,
                    borderLeftColor = Color.gray,
                    borderRightColor = Color.gray,
                    marginLeft = 5,
                    marginTop = 5
                }
            };

            // トグル群
            var toggleContainer = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            var logToggle = new Toggle("Log") { value = s_showLog };
            var warningToggle = new Toggle("Warning") { value = s_showWarning };
            var errorToggle = new Toggle("Error") { value = s_showError };

            void OnToggleChange(ChangeEvent<bool> _)
            {
                s_showLog = logToggle.value;
                s_showWarning = warningToggle.value;
                s_showError = errorToggle.value;
                RefreshList();
            }

            logToggle.RegisterValueChangedCallback(OnToggleChange);
            warningToggle.RegisterValueChangedCallback(OnToggleChange);
            errorToggle.RegisterValueChangedCallback(OnToggleChange);
            toggleContainer.Add(logToggle);
            toggleContainer.Add(warningToggle);
            toggleContainer.Add(errorToggle);
            container.Add(toggleContainer);

            // 検索フィールド
            var searchField = new TextField("Search");
            container.Add(searchField);

            // クリアボタン
            var clearBtn = new Button { text = "Clear" };
            clearBtn.clicked += () =>
            {
                s_logs.Clear();
                if (s_listView != null)
                {
                    s_listView.itemsSource = s_logs;
                    s_listView.Rebuild();
                }
            };
            container.Add(clearBtn);

            // リストビュー
            s_listView = new ListView(s_logs, 20, () => new Label(), (ve, i) =>
                {
                    var label = ve as Label;
                    var entry = s_logs[i];
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

            // 検索コールバック
            searchField.RegisterValueChangedCallback(evt =>
            {
                s_searchQuery = evt.newValue.ToLowerInvariant();
                RefreshList();
            });
            container.Add(s_listView);
            root.Add(container);

            return container;
        }

        private static void RefreshList()
        {
            if (s_listView == null) return;
            var filtered = new List<(string message, string stackTrace, LogType type)>();
            foreach (var log in s_logs)
            {
                if ((log.type == LogType.Log && !s_showLog) ||
                    (log.type == LogType.Warning && !s_showWarning) ||
                    (log.type == LogType.Error && !s_showError))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(s_searchQuery) &&
                    !log.message.ToLowerInvariant().Contains(s_searchQuery))
                {
                    continue;
                }

                filtered.Add(log);
            }

            s_listView.itemsSource = filtered;
            s_listView.Rebuild();
        }
    }
}
