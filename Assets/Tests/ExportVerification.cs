using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DebugToolkit.Tests
{
    /// <summary>
    /// Simple verification script to test export functionality without Unity editor.
    /// This simulates what the export would produce.
    /// </summary>
    public static class ExportVerification
    {
        public static void SimulateExport()
        {
            Debug.Log("=== Debug Export Verification ===");
            
            // Create a mock root element with some test data
            var mockData = CreateMockExportData();
            
            // Test JSON serialization
            var json = SerializeToJsonMock(mockData);
            Debug.Log("JSON Export Preview:");
            Debug.Log(json);
            
            Debug.Log("\n" + new string('=', 50) + "\n");
            
            // Test XML serialization
            var xml = SerializeToXmlMock(mockData);
            Debug.Log("XML Export Preview:");
            Debug.Log(xml);
            
            Debug.Log("=== Verification Complete ===");
        }

        private static Dictionary<string, object> CreateMockExportData()
        {
            return new Dictionary<string, object>
            {
                ["Export Demo"] = new Dictionary<string, object>
                {
                    ["Slider_VolumeSlider"] = 75.5f,
                    ["SliderInt_QualitySlider"] = 7,
                    ["Toggle_VsyncToggle"] = true,
                    ["TextField_PlayerNameField"] = "TestPlayer123",
                    ["DropdownField_DifficultyDropdown"] = "Normal",
                    ["MinMaxSlider_SpawnRangeSlider"] = new { min = 25.0f, max = 75.0f }
                },
                ["TestWindow"] = new Dictionary<string, object>
                {
                    ["Toggle_TestToggle"] = false,
                    ["TextField_TestField"] = "Hello World",
                    ["Slider_TestSlider"] = 42.3f
                }
            };
        }

        private static string SerializeToJsonMock(Dictionary<string, object> data)
        {
            return @"{
  ""exportTimestamp"": ""2024-09-01 10:58:45"",
  ""debugValues"": {
    ""Export Demo"": {
      ""Slider_VolumeSlider"": 75.500,
      ""SliderInt_QualitySlider"": 7,
      ""Toggle_VsyncToggle"": true,
      ""TextField_PlayerNameField"": ""TestPlayer123"",
      ""DropdownField_DifficultyDropdown"": ""Normal"",
      ""MinMaxSlider_SpawnRangeSlider"": { ""min"": 25.000, ""max"": 75.000 }
    },
    ""TestWindow"": {
      ""Toggle_TestToggle"": false,
      ""TextField_TestField"": ""Hello World"",
      ""Slider_TestSlider"": 42.300
    }
  }
}";
        }

        private static string SerializeToXmlMock(Dictionary<string, object> data)
        {
            return @"<?xml version=""1.0"" encoding=""UTF-8""?>
<DebugExport>
  <ExportTimestamp>2024-09-01 10:58:45</ExportTimestamp>
  <DebugValues>
    <Window name=""Export Demo"">
      <Slider_VolumeSlider type=""float"">75.500</Slider_VolumeSlider>
      <SliderInt_QualitySlider type=""integer"">7</SliderInt_QualitySlider>
      <Toggle_VsyncToggle type=""boolean"">true</Toggle_VsyncToggle>
      <TextField_PlayerNameField type=""string"">TestPlayer123</TextField_PlayerNameField>
      <DropdownField_DifficultyDropdown type=""string"">Normal</DropdownField_DifficultyDropdown>
      <MinMaxSlider_SpawnRangeSlider type=""object"">
        <min type=""float"">25.000</min>
        <max type=""float"">75.000</max>
      </MinMaxSlider_SpawnRangeSlider>
    </Window>
    <Window name=""TestWindow"">
      <Toggle_TestToggle type=""boolean"">false</Toggle_TestToggle>
      <TextField_TestField type=""string"">Hello World</TextField_TestField>
      <Slider_TestSlider type=""float"">42.300</Slider_TestSlider>
    </Window>
  </DebugValues>
</DebugExport>";
        }
    }
}