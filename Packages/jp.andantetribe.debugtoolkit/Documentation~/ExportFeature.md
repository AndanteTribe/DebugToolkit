# Debug Export Feature

This feature allows you to export debug menu values to JSON or XML format and copy them to the clipboard.

## Usage

### Adding Export Buttons to Debug Windows

```csharp
// Add both JSON and XML export buttons
window.AddExportButtons();

// Or add individual buttons
window.AddJsonExportButton();
window.AddXmlExportButton();
```

### Direct Export

```csharp
// Export to JSON
DebugExportUtils.ExportToJson(rootElement);

// Export to XML
DebugExportUtils.ExportToXml(rootElement);
```

## Example Output

### JSON Format

```json
{
  "exportTimestamp": "2024-09-01 10:58:45",
  "debugValues": {
    "Export Demo": {
      "Slider_VolumeSlider": 75.500,
      "SliderInt_QualitySlider": 7,
      "Toggle_VsyncToggle": true,
      "TextField_PlayerNameField": "TestPlayer123",
      "DropdownField_DifficultyDropdown": "Normal",
      "MinMaxSlider_SpawnRangeSlider": { "min": 25.000, "max": 75.000 }
    }
  }
}
```

### XML Format

```xml
<?xml version="1.0" encoding="UTF-8"?>
<DebugExport>
  <ExportTimestamp>2024-09-01 10:58:45</ExportTimestamp>
  <DebugValues>
    <Window name="Export Demo">
      <Slider_VolumeSlider type="float">75.500</Slider_VolumeSlider>
      <SliderInt_QualitySlider type="integer">7</SliderInt_QualitySlider>
      <Toggle_VsyncToggle type="boolean">true</Toggle_VsyncToggle>
      <TextField_PlayerNameField type="string">TestPlayer123</TextField_PlayerNameField>
      <DropdownField_DifficultyDropdown type="string">Normal</DropdownField_DifficultyDropdown>
      <MinMaxSlider_SpawnRangeSlider type="object">
        <min type="float">25.000</min>
        <max type="float">75.000</max>
      </MinMaxSlider_SpawnRangeSlider>
    </Window>
  </DebugValues>
</DebugExport>
```

## Supported UI Elements

- **Slider** - exports current value as float
- **SliderInt** - exports current value as integer
- **Toggle** - exports current value as boolean
- **TextField** - exports current text value as string
- **DropdownField** - exports selected value as string
- **EnumField** - exports selected enum value as string
- **RadioButton** - exports current value as boolean
- **RadioButtonGroup** - exports selected index as integer
- **MinMaxSlider** - exports as object with min/max values

## Key Generation

The export system generates keys for UI elements in the following priority:
1. Element's `name` property
2. Element's `label` property (cleaned of spaces and colons)
3. Child label text (cleaned of spaces and colons)
4. Element's `text` property (cleaned of spaces and colons)
5. Hash code as fallback

## Notes

- Exported data is automatically copied to the system clipboard using `GUIUtility.systemCopyBuffer`
- No files are created - everything goes to clipboard as requested
- Export includes timestamp for reference
- Works with all debug windows in the current UI hierarchy
- Non-destructive - doesn't affect existing debug functionality