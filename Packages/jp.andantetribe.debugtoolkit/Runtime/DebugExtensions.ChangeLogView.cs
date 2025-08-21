#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DebugToolkit
{
    public static partial class DebugExtensions
    {
        /// <summary>
        /// Represents a single change log entry
        /// </summary>
        public struct ChangeLogEntry
        {
            public string elementName;
            public string elementType;
            public string previousValue;
            public string newValue;
            public DateTime timestamp;

            public ChangeLogEntry(string elementName, string elementType, string previousValue, string newValue)
            {
                this.elementName = elementName;
                this.elementType = elementType;
                this.previousValue = previousValue;
                this.newValue = newValue;
                this.timestamp = DateTime.Now;
            }
        }

        private static readonly List<ChangeLogEntry> _changeHistory = new();

        /// <summary>
        /// Logs a change to the change history
        /// </summary>
        /// <param name="elementName">Name or label of the UI element</param>
        /// <param name="elementType">Type of the UI element</param>
        /// <param name="previousValue">Previous value as string</param>
        /// <param name="newValue">New value as string</param>
        public static void LogChange(string elementName, string elementType, string previousValue, string newValue)
        {
            var entry = new ChangeLogEntry(elementName, elementType, previousValue, newValue);
            _changeHistory.Add(entry);
        }

        /// <summary>
        /// Adds a change log view to display tracked UI element changes
        /// </summary>
        /// <param name="root">The parent element to add the change log view to</param>
        /// <returns>The created change log view element</returns>
        public static VisualElement AddChangeLogView(this VisualElement root)
        {
            var searchQuery = "";
            var listView = new ListView();

            var changeLogView = new VisualElement();
            changeLogView.AddToClassList(DebugConst.ClassName + "__change-log-view");

            // Search Field
            var searchField = new TextField("Search");
            searchField.RegisterValueChangedCallback(evt =>
            {
                searchQuery = evt.newValue.ToLowerInvariant();
                RefreshChangeList();
            });
            changeLogView.Add(searchField);

            // Clear button
            var clearButton = new Button(() =>
            {
                _changeHistory.Clear();
                RefreshChangeList();
            })
            {
                text = "Clear Changes"
            };
            changeLogView.Add(clearButton);

            // Configure ListView
            listView.makeItem = () => new Label();
            listView.bindItem = (element, i) =>
            {
                var label = (Label)element;
                var filteredItems = GetFilteredChangeHistory(searchQuery);
                
                if (i >= 0 && i < filteredItems.Count)
                {
                    var entry = filteredItems[i];
                    var timeStr = entry.timestamp.ToString("HH:mm:ss");
                    label.text = $"[{timeStr}] {entry.elementType}: \"{entry.elementName}\" changed from \"{entry.previousValue}\" to \"{entry.newValue}\"";
                    
                    // Color coding based on element type
                    switch (entry.elementType.ToLowerInvariant())
                    {
                        case "toggle":
                            label.style.backgroundColor = new StyleColor(new Color(0.7f, 0.9f, 1f, 0.3f));
                            break;
                        case "slider":
                        case "sliderint":
                            label.style.backgroundColor = new StyleColor(new Color(0.9f, 1f, 0.7f, 0.3f));
                            break;
                        case "textfield":
                            label.style.backgroundColor = new StyleColor(new Color(1f, 0.9f, 0.7f, 0.3f));
                            break;
                        case "dropdownfield":
                        case "enumfield":
                            label.style.backgroundColor = new StyleColor(new Color(1f, 0.7f, 0.9f, 0.3f));
                            break;
                        default:
                            label.style.backgroundColor = new StyleColor(new Color(0.9f, 0.9f, 0.9f, 0.3f));
                            break;
                    }
                }
                else
                {
                    label.text = "";
                    label.style.backgroundColor = new StyleColor(Color.white);
                }
            };

            listView.style.flexGrow = 1;
            changeLogView.Add(listView);

            void RefreshChangeList()
            {
                var filteredItems = GetFilteredChangeHistory(searchQuery);
                listView.itemsSource = filteredItems;
                listView.Rebuild();
            }

            // Initial refresh
            RefreshChangeList();

            root.Add(changeLogView);
            return changeLogView;
        }

        private static List<ChangeLogEntry> GetFilteredChangeHistory(string searchQuery)
        {
            var filtered = new List<ChangeLogEntry>();
            
            for (int i = _changeHistory.Count - 1; i >= 0; i--) // Reverse order to show latest first
            {
                var entry = _changeHistory[i];
                
                if (string.IsNullOrEmpty(searchQuery) ||
                    entry.elementName.ToLowerInvariant().Contains(searchQuery) ||
                    entry.elementType.ToLowerInvariant().Contains(searchQuery) ||
                    entry.previousValue.ToLowerInvariant().Contains(searchQuery) ||
                    entry.newValue.ToLowerInvariant().Contains(searchQuery))
                {
                    filtered.Add(entry);
                }
            }
            
            return filtered;
        }
    }
}