#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using LogEntry = System.ValueTuple<string, string, UnityEngine.LogType, System.DateTime>;

namespace DebugToolkit
{
#if UNITY_2023_2_OR_NEWER
    [UxmlElement]
    internal sealed partial class ConsoleView : VisualElement
    {
#else
    internal sealed class ConsoleView : VisualElement
    {
        /// <summary>
        /// Factory class for UXML elements to register in UIBuilder's Library.
        /// </summary>
        public class ConsoleViewFactory : UxmlFactory<ConsoleView, UxmlTraits>
        {
        }
#endif

        private static readonly DateTime s_startTime = DateTime.Now;
        private static readonly Stopwatch s_stopwatch = Stopwatch.StartNew();
        private static readonly ConcurrentQueue<LogEntry> s_logs = new();
        private static int s_logVersion;

        private readonly StringBuilder _stringBuilderCache = new();
        private readonly List<LogEntry> _filteredLogs = s_logs.ToList();
        private int _version = s_logVersion;
        private DateTime _startTime = s_startTime;

        private string _searchQuery = "";
        private bool _showLog = true;
        private bool _showWarning = true;
        private bool _showError = true;
        private bool _showStackTrace = false;
        private bool _showTimestamp = true;

        public ConsoleView()
        {
            AddToClassList(DebugConst.ClassName + "__console-view");
            CreateViewGUI(this);
        }

        public static void Initialize()
        {
            s_logs.Clear();
            s_logVersion = 0;

#if UNITY_EDITOR
            // To avoid multiple subscriptions in the editor
            Application.logMessageReceivedThreaded -= OnApplicationOnLogMessageReceivedThreaded;
#endif
            Application.logMessageReceivedThreaded += OnApplicationOnLogMessageReceivedThreaded;

            static void OnApplicationOnLogMessageReceivedThreaded(string message, string stackTrace, LogType type)
            {
                s_logs.Enqueue((message, stackTrace, type, GetCurrentTimestamp()));
                Interlocked.Increment(ref s_logVersion);
            }
        }

        private static void OnBindItem(ConsoleView self, VisualElement element, LogEntry entry)
        {
            var (message, stackTrace, type, timeStamp) = entry;
            element.style.backgroundColor = type switch
            {
                LogType.Warning => DebugConst.StyleColor.Warning,
                LogType.Error or LogType.Assert or LogType.Exception => DebugConst.StyleColor.Error,
                _ => DebugConst.StyleColor.White,
            };

            var label = (Label)element;
            var sb = self._stringBuilderCache.Clear();
            sb.Append(type switch
            {
                LogType.Error => "[Error] ",
                LogType.Assert => "[Assert] ",
                LogType.Warning => "[Warning] ",
                LogType.Exception => "[Exception] ",
                _ => "[Log] ",
            });
            if (self._showTimestamp)
            {
                sb.Append('[');
                var buffer = (Span<char>)stackalloc char[12];
                timeStamp.TryFormat(buffer, out _, "HH:mm:ss.fff");
                sb.Append(buffer);
                sb.Append(']').Append(' ');
            }

            sb.Append(message);
            if (self._showStackTrace)
            {
                sb.AppendLine();
                sb.Append(stackTrace);
            }

            label.text = sb.ToString();
        }

        private static void CreateViewGUI(ConsoleView self)
        {
            var listView = new ListView(self._filteredLogs, makeItem: static () => new Label(), bindItem: static (_, _) => { });
            listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            listView.SetViewController(new BindListViewController<LogEntry, ConsoleView>(
                self, static (view, element, arg3) => OnBindItem(view, element, arg3)));
            listView.itemsSource = self._filteredLogs;

            // every per frame update _filterLogs from s_logs
            self.schedule.Execute(() =>
            {
                if (self._version != s_logVersion)
                {
                    self.UpdateFilteredLogs(true);
                    listView.Rebuild();
                }
            }).Every(0);

            var menubar = new VisualElement() { style = { flexDirection = FlexDirection.Row, minHeight = 60} };
            self.Add(menubar);

            var logToggle = new Toggle(nameof(LogType.Log)) { value = self._showLog };
            logToggle.RegisterCallback<ChangeEvent<bool>, (ConsoleView self, ListView listview)>(static (evt, args) =>
            {
                if (evt.previousValue != evt.newValue)
                {
                    args.self._showLog = evt.newValue;
                    args.self.UpdateFilteredLogs();
                    args.listview.Rebuild();
                }
            }, (self, listView));
            menubar.Add(logToggle);

            var warningToggle = new Toggle(nameof(LogType.Warning)) { value = self._showWarning };
            warningToggle.RegisterCallback<ChangeEvent<bool>, (ConsoleView self, ListView listview)>(static (evt, args) =>
            {
                if (evt.previousValue != evt.newValue)
                {
                    args.self._showWarning = evt.newValue;
                    args.self.UpdateFilteredLogs();
                    args.listview.Rebuild();
                }
            }, (self, listView));
            menubar.Add(warningToggle);

            var errorToggle = new Toggle(nameof(LogType.Error)) { value = self._showError };
            errorToggle.RegisterCallback<ChangeEvent<bool>, (ConsoleView self, ListView listview)>(static (evt, args) =>
            {
                if (evt.previousValue != evt.newValue)
                {
                    args.self._showError = evt.newValue;
                    args.self.UpdateFilteredLogs();
                    args.listview.Rebuild();
                }
            }, (self, listView));
            menubar.Add(errorToggle);

            var stackTraceToggle = new Toggle("StackTrace") { value = self._showStackTrace };
            stackTraceToggle.RegisterCallback<ChangeEvent<bool>, (ConsoleView self, ListView listview)>(static (evt, args) =>
            {
                if (evt.previousValue != evt.newValue)
                {
                    args.self._showStackTrace = evt.newValue;
                    args.listview.Rebuild();
                }
            }, (self, listView));
            menubar.Add(stackTraceToggle);

            var timestampToggle = new Toggle("Timestamp") { value = self._showTimestamp };
            timestampToggle.RegisterCallback<ChangeEvent<bool>, (ConsoleView self, ListView listview)>(static (evt, args) =>
            {
                if (evt.previousValue != evt.newValue)
                {
                    args.self._showTimestamp = evt.newValue;
                    args.listview.Rebuild();
                }
            }, (self, listView));
            menubar.Add(timestampToggle);

            var searchField = new TextField();
            searchField.RegisterCallback<ChangeEvent<string>, (ConsoleView self, ListView listview)>(static (evt, args) =>
            {
                if (evt.previousValue != evt.newValue)
                {
                    args.self._searchQuery = evt.newValue;
                    args.self.UpdateFilteredLogs();
                    args.listview.Rebuild();
                }
            }, (self, listView));
            self.Add(searchField);

            var clearBtn = new Button() { text = "Clear" };
            clearBtn.RegisterCallback<ClickEvent, (ConsoleView self, ListView listview)>(static (_, args) =>
            {
                args.self._startTime = GetCurrentTimestamp();
                args.self.UpdateFilteredLogs();
                args.listview.Rebuild();
            }, (self, listView));
            self.Add(clearBtn);

            self.Add(listView);
        }

        private void UpdateFilteredLogs(bool onlyDiff = false)
        {
            var logs = (IEnumerable<LogEntry>)s_logs;
            if (onlyDiff)
            {
                logs = s_logs.Skip(_version);
            }
            else
            {
                _filteredLogs.Clear();
            }
            foreach (var (message, stacktrace, type, timestamp) in logs)
            {
                if ((!_showLog && type is LogType.Log) ||
                    (!_showWarning && type is LogType.Warning) ||
                    (!_showError && type is LogType.Error or LogType.Assert or LogType.Exception) ||
                    (!string.IsNullOrEmpty(_searchQuery) && !message.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    timestamp < _startTime)
                {
                    continue;
                }
                _filteredLogs.Add((message, stacktrace, type, timestamp));
            }
            _version = s_logVersion;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DateTime GetCurrentTimestamp() => s_startTime + s_stopwatch.Elapsed;
    }
}