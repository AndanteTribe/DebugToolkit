#nullable enable

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace DebugToolkit
{
    /// <summary>
    /// Utility class for exporting debug menu values to JSON or XML format.
    /// Exports are sent to the system clipboard using GUIUtility.systemCopyBuffer.
    /// </summary>
    public static class DebugExportUtils
    {
        /// <summary>
        /// Exports all debug window values to JSON format and copies to clipboard.
        /// </summary>
        /// <param name="root">The root visual element containing debug windows</param>
        public static void ExportToJson(VisualElement root)
        {
            var exportData = CollectDebugValues(root);
            var json = SerializeToJson(exportData);
            CopyToClipboard(json);
            Debug.Log("Debug values exported to JSON and copied to clipboard.");
        }

        /// <summary>
        /// Exports all debug window values to XML format and copies to clipboard.
        /// </summary>
        /// <param name="root">The root visual element containing debug windows</param>
        public static void ExportToXml(VisualElement root)
        {
            var exportData = CollectDebugValues(root);
            var xml = SerializeToXml(exportData);
            CopyToClipboard(xml);
            Debug.Log("Debug values exported to XML and copied to clipboard.");
        }

        /// <summary>
        /// Collects all debug values from UI elements in debug windows.
        /// </summary>
        /// <param name="root">The root visual element</param>
        /// <returns>Dictionary containing element names and their values</returns>
        private static Dictionary<string, object> CollectDebugValues(VisualElement root)
        {
            var values = new Dictionary<string, object>();
            var debugWindows = root.Query<DebugWindow>().ToList();

            foreach (var window in debugWindows)
            {
                var windowName = window.name ?? "UnnamedWindow";
                var windowValues = new Dictionary<string, object>();

                // Collect values from various UI elements
                CollectSliderValues(window, windowValues);
                CollectToggleValues(window, windowValues);
                CollectTextFieldValues(window, windowValues);
                CollectDropdownValues(window, windowValues);
                CollectEnumFieldValues(window, windowValues);
                CollectRadioButtonValues(window, windowValues);
                CollectMinMaxSliderValues(window, windowValues);

                if (windowValues.Count > 0)
                {
                    values[windowName] = windowValues;
                }
            }

            return values;
        }

        /// <summary>
        /// Collects values from Slider elements.
        /// </summary>
        private static void CollectSliderValues(VisualElement container, Dictionary<string, object> values)
        {
            var sliders = container.Query<Slider>().ToList();
            foreach (var slider in sliders)
            {
                var key = GetElementKey(slider, "Slider");
                values[key] = slider.value;
            }

            var sliderInts = container.Query<SliderInt>().ToList();
            foreach (var sliderInt in sliderInts)
            {
                var key = GetElementKey(sliderInt, "SliderInt");
                values[key] = sliderInt.value;
            }
        }

        /// <summary>
        /// Collects values from Toggle elements.
        /// </summary>
        private static void CollectToggleValues(VisualElement container, Dictionary<string, object> values)
        {
            var toggles = container.Query<Toggle>().ToList();
            foreach (var toggle in toggles)
            {
                var key = GetElementKey(toggle, "Toggle");
                values[key] = toggle.value;
            }
        }

        /// <summary>
        /// Collects values from TextField elements.
        /// </summary>
        private static void CollectTextFieldValues(VisualElement container, Dictionary<string, object> values)
        {
            var textFields = container.Query<TextField>().ToList();
            foreach (var textField in textFields)
            {
                var key = GetElementKey(textField, "TextField");
                values[key] = textField.value ?? string.Empty;
            }
        }

        /// <summary>
        /// Collects values from DropdownField elements.
        /// </summary>
        private static void CollectDropdownValues(VisualElement container, Dictionary<string, object> values)
        {
            var dropdowns = container.Query<DropdownField>().ToList();
            foreach (var dropdown in dropdowns)
            {
                var key = GetElementKey(dropdown, "DropdownField");
                values[key] = dropdown.value ?? string.Empty;
            }
        }

        /// <summary>
        /// Collects values from EnumField elements.
        /// </summary>
        private static void CollectEnumFieldValues(VisualElement container, Dictionary<string, object> values)
        {
            var enumFields = container.Query<EnumField>().ToList();
            foreach (var enumField in enumFields)
            {
                var key = GetElementKey(enumField, "EnumField");
                values[key] = enumField.value?.ToString() ?? string.Empty;
            }
        }

        /// <summary>
        /// Collects values from RadioButton and RadioButtonGroup elements.
        /// </summary>
        private static void CollectRadioButtonValues(VisualElement container, Dictionary<string, object> values)
        {
            var radioButtons = container.Query<RadioButton>().ToList();
            foreach (var radioButton in radioButtons)
            {
                var key = GetElementKey(radioButton, "RadioButton");
                values[key] = radioButton.value;
            }

            var radioButtonGroups = container.Query<RadioButtonGroup>().ToList();
            foreach (var radioButtonGroup in radioButtonGroups)
            {
                var key = GetElementKey(radioButtonGroup, "RadioButtonGroup");
                values[key] = radioButtonGroup.value;
            }
        }

        /// <summary>
        /// Collects values from MinMaxSlider elements.
        /// </summary>
        private static void CollectMinMaxSliderValues(VisualElement container, Dictionary<string, object> values)
        {
            var minMaxSliders = container.Query<MinMaxSlider>().ToList();
            foreach (var minMaxSlider in minMaxSliders)
            {
                var key = GetElementKey(minMaxSlider, "MinMaxSlider");
                values[key] = new { min = minMaxSlider.value.x, max = minMaxSlider.value.y };
            }
        }

        /// <summary>
        /// Gets a unique key for a UI element based on its name, label, or text.
        /// </summary>
        private static string GetElementKey(VisualElement element, string elementType)
        {
            // Try to get a meaningful name from various sources
            if (!string.IsNullOrEmpty(element.name))
                return $"{elementType}_{element.name}";

            // Try to get label text for labeled elements
            try
            {
                // Use reflection to check if element has a label property
                var labelProperty = element.GetType().GetProperty("label");
                if (labelProperty != null)
                {
                    var labelValue = labelProperty.GetValue(element) as string;
                    if (!string.IsNullOrEmpty(labelValue))
                        return $"{elementType}_{labelValue.Replace(" ", "_").Replace(":", "")}";
                }
            }
            catch
            {
                // Ignore reflection errors
            }

            // Try to find child label elements
            var label = element.Q<Label>();
            if (label != null && !string.IsNullOrEmpty(label.text))
                return $"{elementType}_{label.text.Replace(" ", "_").Replace(":", "")}";

            // Try to get text from the element itself if it has text
            try
            {
                var textProperty = element.GetType().GetProperty("text");
                if (textProperty != null)
                {
                    var textValue = textProperty.GetValue(element) as string;
                    if (!string.IsNullOrEmpty(textValue))
                        return $"{elementType}_{textValue.Replace(" ", "_").Replace(":", "")}";
                }
            }
            catch
            {
                // Ignore reflection errors
            }

            // Fall back to a generated name
            return $"{elementType}_{element.GetHashCode()}";
        }

        /// <summary>
        /// Serializes data to JSON format.
        /// </summary>
        private static string SerializeToJson(Dictionary<string, object> data)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  \"exportTimestamp\": \"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\",");
            sb.AppendLine("  \"debugValues\": {");

            var windowEntries = new List<string>();
            foreach (var kvp in data)
            {
                var windowJson = SerializeWindowToJson(kvp.Key, kvp.Value);
                windowEntries.Add(windowJson);
            }

            sb.AppendLine(string.Join(",\n", windowEntries));
            sb.AppendLine("  }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Serializes a window's data to JSON format.
        /// </summary>
        private static string SerializeWindowToJson(string windowName, object windowData)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"    \"{EscapeJson(windowName)}\": {{");

            if (windowData is Dictionary<string, object> values)
            {
                var entries = new List<string>();
                foreach (var kvp in values)
                {
                    entries.Add($"      \"{EscapeJson(kvp.Key)}\": {SerializeValueToJson(kvp.Value)}");
                }
                sb.AppendLine(string.Join(",\n", entries));
            }

            sb.Append("    }");
            return sb.ToString();
        }

        /// <summary>
        /// Serializes a value to JSON format.
        /// </summary>
        private static string SerializeValueToJson(object value)
        {
            if (value == null)
                return "null";
            
            if (value is bool b)
                return b.ToString().ToLower();
                
            if (value is string s)
                return $"\"{EscapeJson(s)}\"";
                
            if (value is float f)
                return f.ToString("F3");
                
            if (value is double d)
                return d.ToString("F3");
                
            if (value is int i)
                return i.ToString();
                
            // Handle anonymous objects (like MinMaxSlider values)
            var type = value.GetType();
            if (type.Name.Contains("AnonymousType") || type.IsClass && !type.Namespace?.StartsWith("System") == true)
            {
                return SerializeAnonymousObjectToJson(value);
            }
            
            return $"\"{EscapeJson(value.ToString() ?? "")}\"";
        }

        /// <summary>
        /// Serializes anonymous objects (like MinMaxSlider values) to JSON.
        /// </summary>
        private static string SerializeAnonymousObjectToJson(object obj)
        {
            var sb = new StringBuilder();
            sb.Append("{ ");

            var properties = obj.GetType().GetProperties();
            var entries = new List<string>();

            foreach (var prop in properties)
            {
                var propValue = prop.GetValue(obj);
                entries.Add($"\"{prop.Name}\": {SerializeValueToJson(propValue)}");
            }

            sb.Append(string.Join(", ", entries));
            sb.Append(" }");
            return sb.ToString();
        }

        /// <summary>
        /// Serializes data to XML format.
        /// </summary>
        private static string SerializeToXml(Dictionary<string, object> data)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<DebugExport>");
            sb.AppendLine($"  <ExportTimestamp>{DateTime.Now:yyyy-MM-dd HH:mm:ss}</ExportTimestamp>");
            sb.AppendLine("  <DebugValues>");

            foreach (var kvp in data)
            {
                sb.AppendLine($"    <Window name=\"{EscapeXml(kvp.Key)}\">");

                if (kvp.Value is Dictionary<string, object> values)
                {
                    foreach (var valueKvp in values)
                    {
                        SerializeValueToXml(sb, valueKvp.Key, valueKvp.Value, 6);
                    }
                }

                sb.AppendLine("    </Window>");
            }

            sb.AppendLine("  </DebugValues>");
            sb.AppendLine("</DebugExport>");

            return sb.ToString();
        }

        /// <summary>
        /// Serializes a value to XML format.
        /// </summary>
        private static void SerializeValueToXml(StringBuilder sb, string key, object value, int indent)
        {
            var indentStr = new string(' ', indent);

            if (value == null)
            {
                sb.AppendLine($"{indentStr}<{EscapeXml(key)} type=\"null\" />");
            }
            else if (value is bool b)
            {
                sb.AppendLine($"{indentStr}<{EscapeXml(key)} type=\"boolean\">{b.ToString().ToLower()}</{EscapeXml(key)}>");
            }
            else if (value is string s)
            {
                sb.AppendLine($"{indentStr}<{EscapeXml(key)} type=\"string\">{EscapeXml(s)}</{EscapeXml(key)}>");
            }
            else if (value is float f)
            {
                sb.AppendLine($"{indentStr}<{EscapeXml(key)} type=\"float\">{f:F3}</{EscapeXml(key)}>");
            }
            else if (value is double d)
            {
                sb.AppendLine($"{indentStr}<{EscapeXml(key)} type=\"double\">{d:F3}</{EscapeXml(key)}>");
            }
            else if (value is int i)
            {
                sb.AppendLine($"{indentStr}<{EscapeXml(key)} type=\"integer\">{i}</{EscapeXml(key)}>");
            }
            else
            {
                var type = value.GetType();
                if (type.Name.Contains("AnonymousType") || (type.IsClass && !type.Namespace?.StartsWith("System") == true))
                {
                    sb.AppendLine($"{indentStr}<{EscapeXml(key)} type=\"object\">");
                    var properties = type.GetProperties();
                    foreach (var prop in properties)
                    {
                        var propValue = prop.GetValue(value);
                        SerializeValueToXml(sb, prop.Name, propValue, indent + 2);
                    }
                    sb.AppendLine($"{indentStr}</{EscapeXml(key)}>");
                }
                else
                {
                    sb.AppendLine($"{indentStr}<{EscapeXml(key)} type=\"string\">{EscapeXml(value.ToString() ?? "")}</{EscapeXml(key)}>");
                }
            }
        }

        /// <summary>
        /// Escapes special characters for JSON strings.
        /// </summary>
        private static string EscapeJson(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }

        /// <summary>
        /// Escapes special characters for XML.
        /// </summary>
        private static string EscapeXml(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }

        /// <summary>
        /// Copies text to the system clipboard.
        /// </summary>
        private static void CopyToClipboard(string text)
        {
            GUIUtility.systemCopyBuffer = text;
        }
    }
}