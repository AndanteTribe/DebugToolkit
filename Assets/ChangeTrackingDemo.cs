using UnityEngine;
using UnityEngine.UIElements;

namespace DebugToolkit
{
    /// <summary>
    /// Demo script to showcase the change visualization feature
    /// </summary>
    public class ChangeTrackingDemo : DebugViewerBase
    {
        protected override VisualElement CreateViewGUI()
        {
            var root = base.CreateViewGUI();
            
            // Create demo window
            var demoWindow = root.AddWindow("Change Tracking Demo");
            var (tabRoot, demoTab) = demoWindow.AddTab("Demo Controls");
            
            // Add various UI elements with change tracking
            var instructions = new Label("Interact with the controls below to see changes tracked in the Change Log window:");
            instructions.style.whiteSpace = WhiteSpace.Normal;
            instructions.style.fontSize = 14;
            instructions.style.marginBottom = 10;
            demoTab.Add(instructions);
            
            // Toggle demo
            var demoToggle = new Toggle("Demo Toggle");
            demoToggle.RegisterValueChangedCallbackWithTracking(evt =>
            {
                Debug.Log($"Toggle changed to: {evt.newValue}");
            });
            demoTab.Add(demoToggle);
            
            // Slider demo
            var demoSlider = new Slider(0, 100) { label = "Demo Slider", value = 50 };
            demoSlider.RegisterValueChangedCallbackWithTracking(evt =>
            {
                Debug.Log($"Slider changed to: {evt.newValue:F1}");
            });
            demoTab.Add(demoSlider);
            
            // Text field demo
            var demoTextField = new TextField("Demo Text Field") { value = "Initial text" };
            demoTextField.RegisterValueChangedCallbackWithTracking(evt =>
            {
                Debug.Log($"Text field changed to: {evt.newValue}");
            });
            demoTab.Add(demoTextField);
            
            // Dropdown demo
            var demoDropdown = new DropdownField("Demo Dropdown");
            demoDropdown.choices = new System.Collections.Generic.List<string> { "Option A", "Option B", "Option C" };
            demoDropdown.value = "Option A";
            demoDropdown.RegisterValueChangedCallbackWithTracking(evt =>
            {
                Debug.Log($"Dropdown changed to: {evt.newValue}");
            });
            demoTab.Add(demoDropdown);
            
            // Integer slider demo
            var demoSliderInt = new SliderInt(1, 10) { label = "Demo Int Slider", value = 5 };
            demoSliderInt.RegisterValueChangedCallbackWithTracking(evt =>
            {
                Debug.Log($"Int slider changed to: {evt.newValue}");
            });
            demoTab.Add(demoSliderInt);
            
            // Create the change log window
            var changeLogWindow = root.AddWindow("Change Log");
            changeLogWindow.AddChangeLogView();
            
            return root;
        }
    }
}