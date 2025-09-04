using UnityEngine;
using UnityEngine.UIElements;

namespace DebugToolkit
{
    /// <summary>
    /// Simple demonstration of the debug export functionality.
    /// This can be used to manually test the export feature.
    /// </summary>
    public class DebugExportDemo : DebugViewerBase
    {
        protected override VisualElement CreateViewGUI()
        {
            var root = base.CreateViewGUI();
            
            // Create a demo window with various UI elements
            var demoWindow = root.AddWindow("Export Demo");
            
            // Add some test UI elements with meaningful names
            var floatSlider = new Slider(0, 100) 
            { 
                name = "VolumeSlider", 
                label = "Master Volume",
                value = 75.5f,
                showInputField = true
            };
            demoWindow.Add(floatSlider);
            
            var intSlider = new SliderInt(1, 10) 
            { 
                name = "QualitySlider", 
                label = "Graphics Quality",
                value = 7,
                showInputField = true
            };
            demoWindow.Add(intSlider);
            
            var enableToggle = new Toggle 
            { 
                name = "VsyncToggle", 
                label = "Enable VSync",
                value = true 
            };
            demoWindow.Add(enableToggle);
            
            var playerNameField = new TextField 
            { 
                name = "PlayerNameField", 
                label = "Player Name",
                value = "TestPlayer123"
            };
            demoWindow.Add(playerNameField);
            
            var difficultyDropdown = new DropdownField 
            { 
                name = "DifficultyDropdown", 
                label = "Game Difficulty",
                choices = new System.Collections.Generic.List<string> { "Easy", "Normal", "Hard", "Expert" },
                value = "Normal"
            };
            demoWindow.Add(difficultyDropdown);
            
            var rangeSlider = new MinMaxSlider("Spawn Range", 0, 100, 25, 75)
            {
                name = "SpawnRangeSlider"
            };
            demoWindow.Add(rangeSlider);
            
            // Add export buttons
            demoWindow.AddExportButtons();
            
            // Add usage instructions
            var instructionsLabel = new Label
            {
                text = "Instructions:\n" +
                       "1. Adjust the values above\n" +
                       "2. Click 'JSON' or 'XML' to export\n" +
                       "3. Values will be copied to clipboard\n" +
                       "4. Paste into a text editor to view the exported data",
                style = { whiteSpace = WhiteSpace.Normal, marginTop = 20 }
            };
            demoWindow.Add(instructionsLabel);
            
            return root;
        }
    }
}