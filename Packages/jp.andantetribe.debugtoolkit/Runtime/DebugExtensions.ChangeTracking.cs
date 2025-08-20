#nullable enable

using System;
using UnityEngine.UIElements;

namespace DebugToolkit
{
    /// <summary>
    /// Extension methods for UI elements to automatically track value changes
    /// </summary>
    public static partial class DebugExtensions
    {
        /// <summary>
        /// Registers a value changed callback that automatically logs changes for a Toggle
        /// </summary>
        /// <param name="toggle">The toggle element</param>
        /// <param name="callback">The callback to execute when value changes</param>
        /// <param name="trackChanges">Whether to track changes in the change log (default: true)</param>
        /// <returns>The toggle element for method chaining</returns>
        public static Toggle RegisterValueChangedCallbackWithTracking(this Toggle toggle, EventCallback<ChangeEvent<bool>> callback, bool trackChanges = true)
        {
            toggle.RegisterValueChangedCallback(evt =>
            {
                if (trackChanges)
                {
                    var elementName = string.IsNullOrEmpty(toggle.label) ? toggle.text : toggle.label;
                    if (string.IsNullOrEmpty(elementName))
                        elementName = toggle.name ?? "Unnamed Toggle";
                    
                    LogChange(elementName, "Toggle", evt.previousValue.ToString(), evt.newValue.ToString());
                }
                callback(evt);
            });
            return toggle;
        }

        /// <summary>
        /// Registers a value changed callback that automatically logs changes for a Slider
        /// </summary>
        /// <param name="slider">The slider element</param>
        /// <param name="callback">The callback to execute when value changes</param>
        /// <param name="trackChanges">Whether to track changes in the change log (default: true)</param>
        /// <returns>The slider element for method chaining</returns>
        public static Slider RegisterValueChangedCallbackWithTracking(this Slider slider, EventCallback<ChangeEvent<float>> callback, bool trackChanges = true)
        {
            slider.RegisterValueChangedCallback(evt =>
            {
                if (trackChanges)
                {
                    var elementName = slider.label ?? slider.name ?? "Unnamed Slider";
                    LogChange(elementName, "Slider", evt.previousValue.ToString("F2"), evt.newValue.ToString("F2"));
                }
                callback(evt);
            });
            return slider;
        }

        /// <summary>
        /// Registers a value changed callback that automatically logs changes for a SliderInt
        /// </summary>
        /// <param name="sliderInt">The slider int element</param>
        /// <param name="callback">The callback to execute when value changes</param>
        /// <param name="trackChanges">Whether to track changes in the change log (default: true)</param>
        /// <returns>The slider int element for method chaining</returns>
        public static SliderInt RegisterValueChangedCallbackWithTracking(this SliderInt sliderInt, EventCallback<ChangeEvent<int>> callback, bool trackChanges = true)
        {
            sliderInt.RegisterValueChangedCallback(evt =>
            {
                if (trackChanges)
                {
                    var elementName = sliderInt.label ?? sliderInt.name ?? "Unnamed SliderInt";
                    LogChange(elementName, "SliderInt", evt.previousValue.ToString(), evt.newValue.ToString());
                }
                callback(evt);
            });
            return sliderInt;
        }

        /// <summary>
        /// Registers a value changed callback that automatically logs changes for a TextField
        /// </summary>
        /// <param name="textField">The text field element</param>
        /// <param name="callback">The callback to execute when value changes</param>
        /// <param name="trackChanges">Whether to track changes in the change log (default: true)</param>
        /// <returns>The text field element for method chaining</returns>
        public static TextField RegisterValueChangedCallbackWithTracking(this TextField textField, EventCallback<ChangeEvent<string>> callback, bool trackChanges = true)
        {
            textField.RegisterValueChangedCallback(evt =>
            {
                if (trackChanges)
                {
                    var elementName = textField.label ?? textField.name ?? "Unnamed TextField";
                    LogChange(elementName, "TextField", evt.previousValue ?? "", evt.newValue ?? "");
                }
                callback(evt);
            });
            return textField;
        }

        /// <summary>
        /// Registers a value changed callback that automatically logs changes for a DropdownField
        /// </summary>
        /// <param name="dropdown">The dropdown field element</param>
        /// <param name="callback">The callback to execute when value changes</param>
        /// <param name="trackChanges">Whether to track changes in the change log (default: true)</param>
        /// <returns>The dropdown field element for method chaining</returns>
        public static DropdownField RegisterValueChangedCallbackWithTracking(this DropdownField dropdown, EventCallback<ChangeEvent<string>> callback, bool trackChanges = true)
        {
            dropdown.RegisterValueChangedCallback(evt =>
            {
                if (trackChanges)
                {
                    var elementName = dropdown.label ?? dropdown.name ?? "Unnamed DropdownField";
                    LogChange(elementName, "DropdownField", evt.previousValue ?? "", evt.newValue ?? "");
                }
                callback(evt);
            });
            return dropdown;
        }

        /// <summary>
        /// Registers a value changed callback that automatically logs changes for an EnumField
        /// </summary>
        /// <param name="enumField">The enum field element</param>
        /// <param name="callback">The callback to execute when value changes</param>
        /// <param name="trackChanges">Whether to track changes in the change log (default: true)</param>
        /// <returns>The enum field element for method chaining</returns>
        public static EnumField RegisterValueChangedCallbackWithTracking(this EnumField enumField, EventCallback<ChangeEvent<Enum>> callback, bool trackChanges = true)
        {
            enumField.RegisterValueChangedCallback(evt =>
            {
                if (trackChanges)
                {
                    var elementName = enumField.label ?? enumField.name ?? "Unnamed EnumField";
                    LogChange(elementName, "EnumField", evt.previousValue?.ToString() ?? "", evt.newValue?.ToString() ?? "");
                }
                callback(evt);
            });
            return enumField;
        }

        /// <summary>
        /// Registers a value changed callback that automatically logs changes for a MinMaxSlider
        /// </summary>
        /// <param name="minMaxSlider">The min max slider element</param>
        /// <param name="callback">The callback to execute when value changes</param>
        /// <param name="trackChanges">Whether to track changes in the change log (default: true)</param>
        /// <returns>The min max slider element for method chaining</returns>
        public static MinMaxSlider RegisterValueChangedCallbackWithTracking(this MinMaxSlider minMaxSlider, EventCallback<ChangeEvent<UnityEngine.Vector2>> callback, bool trackChanges = true)
        {
            minMaxSlider.RegisterValueChangedCallback(evt =>
            {
                if (trackChanges)
                {
                    var elementName = minMaxSlider.label ?? minMaxSlider.name ?? "Unnamed MinMaxSlider";
                    LogChange(elementName, "MinMaxSlider", 
                        $"({evt.previousValue.x:F2}, {evt.previousValue.y:F2})", 
                        $"({evt.newValue.x:F2}, {evt.newValue.y:F2})");
                }
                callback(evt);
            });
            return minMaxSlider;
        }
    }
}