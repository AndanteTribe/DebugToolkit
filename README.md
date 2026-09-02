[![Run the Unity Test](https://github.com/AndanteTribe/DebugToolkit/actions/workflows/unity-test.yml/badge.svg)](https://github.com/AndanteTribe/DebugToolkit/actions/workflows/unity-test.yml)
[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/AndanteTribe/DebugToolkit)
[![Readme_JP](https://img.shields.io/badge/DebugToolkit-Japanese-red)](README_JP.md)
# DebugToolkit

DebugToolkit is a library that makes it easy to build runtime debug menus.

![img.png](Documentation/debugtoolkit.png)

## Overview

DebugToolkit helps you quickly create runtime debug UIs using Unity UIToolkit without writing USS styles. You can build and operate everything with C# scripts only. It provides useful features for development and debugging such as creating draggable debug windows, showing performance information, viewing console logs, and a text field with undo/redo history.

### Concept
DebugToolkit is designed based on the following concepts:

1. Add debug features using C# code only
2. Engineers don’t need to worry about layout and styling
3. Minimal and low dependency

## Quick Start

### Install

Download a Unity package from [Releases](https://github.com/AndanteTribe/DebugToolkit/releases), or add via Package Manager using the following URL:

```
https://github.com/AndanteTribe/DebugToolkit.git?path=Packages/jp.andantetribe.debugtoolkit
```

### Setup

DebugToolkit is enabled by default after installation. No additional setup is required.

**Optional:** If you want to disable DebugToolkit entirely, add `DISABLE_DEBUGTOOLKIT` to Project Settings > Player > Other Settings > Script Compilation > Scripting Define Symbols.

### Basic Usage

1. Create a class that inherits `DebugViewerBase`
2. Override `CreateViewGUI()`
3. Implement your debug menu inside `CreateViewGUI()`
4. Call `Start()` at runtime

### Example

```csharp
#if !DISABLE_DEBUGTOOLKIT
using DebugToolkit;
using UnityEngine;
using UnityEngine.UIElements;

public class MyDebugView : DebugViewerBase
{
    protected override VisualElement CreateViewGUI()
    {
        var root = base.CreateViewGUI();

        // Add performance info label
        root.AddProfileInfoLabel();

        // Add a button
        var button = new Button() { text = "Hoge" };
        button.RegisterCallback<ClickEvent>(_ => Debug.Log("Hoge"));
        root.Add(button);

        return root;
    }
}
#endif
```

```csharp
#if !DISABLE_DEBUGTOOLKIT
using UnityEngine;

public class DebugInitializer : MonoBehaviour
{
    private MyDebugView _debugView;

    void Start()
    {
        _debugView = new MyDebugView();
        // Build the debug menu
        _debugView.Start();
    }
}
#endif
```

![quick-start-example1.png](Documentation/quick-start-example1.png)
![quick-start-example2.png](Documentation/quick-start-example2.png)

### Toggle All Visibility
While using DebugToolkit, a toggle button is shown at the bottom of the screen. Pressing it toggles visibility of all debug windows. You can also restore windows that you’ve hidden.

#### Keyboard shortcut

Set `ToggleAllVisibleKey` to do the same from the keyboard. It also brings back the fixed debug menu
(the master window) once you have hidden it. The default is `KeyCode.None`, which disables the shortcut.

```csharp
#if !DISABLE_DEBUGTOOLKIT
_debugView = new MyDebugView
{
    // Toggle every debug window with F1.
    ToggleAllVisibleKey = KeyCode.F1,

    // Modifier keys, if you need them (e.g. Shift + F1).
    ToggleAllVisibleKeyModifiers = EventModifiers.Shift,
};
_debugView.Start();
#endif
```

The key is picked up by the panel DebugToolkit renders into. If another UI Toolkit panel holds the
keyboard focus, the shortcut does not fire there; call `DebugViewerBase.ToggleAllVisible()` from your
own input handling in that case.

## Samples

You can import `Samples` from Package Manager to try sample scenes and scripts.

## Extension Methods

### `VisualElement AddWindow(this VisualElement root, string windowName)`
Adds a new debug window.
```csharp
#if !DISABLE_DEBUGTOOLKIT
public class MyDebugView : DebugViewerBase
{
    protected override VisualElement CreateViewGUI()
    {
        var root = base.CreateViewGUI();

        var window = root.AddWindow("Window1");
        window.Add(new Label("This is New Window"));

        return root;
    }
}
#endif
```
![window.png](Documentation/window.png)
![window-open.png](Documentation/window-open.png)

### `void AddConsoleView(this VisualElement root)`
Adds a console log viewer. You can check Unity’s console logs at runtime.
```csharp
#if !DISABLE_DEBUGTOOLKIT
public class MyDebugView : DebugViewerBase
{
    protected override VisualElement CreateViewGUI()
    {
        var root = base.CreateViewGUI();

        root.AddConsoleView();

        return root;
    }
}
#endif
```
![console.png](Documentation/console.png)

### `void AddProfileInfoLabel(this VisualElement root)`
Adds a label that shows performance information.
```csharp
#if !DISABLE_DEBUGTOOLKIT
public class MyDebugView : DebugViewerBase
{
    protected override VisualElement CreateViewGUI()
    {
        var root = base.CreateViewGUI();

        root.AddProfileInfoLabel();

        return root;
    }
}
#endif
```
![profile-info-label.png](Documentation/profile-info-label.png)

### Unity 2023.2 or newer

### `ScrollView AddTab(this TabView tabView, string label = "")`
Adds a new tab to an existing TabView.

### `(TabView, ScrollView) AddTab(this VisualElement root, string label = "")`
Adds a TabView and a tab to the target VisualElement.

```csharp
#if !DISABLE_DEBUGTOOLKIT
public class MyDebugView : DebugViewerBase
{
    protected override VisualElement CreateViewGUI()
    {
        var root = base.CreateViewGUI();

        var (tabView, scrollView) = root.AddTab("Tab1");
        tabView.AddTab("Tab2");
        scrollView.Add(new Button(() => { Debug.Log("Button Clicked!"); }) { text = "Click Me" });

        return root;
    }
}
#endif
```
![tab.png](Documentation/tab.png)

## Custom UI Elements

### `HistoryTextField`
A TextField with undo/redo history.
- Undo: `Ctrl or Cmd + Z`
- Redo: `Ctrl or Cmd + Y` or `Ctrl or Cmd + Shift + Z`
```csharp
#if !DISABLE_DEBUGTOOLKIT
public class MyDebugView : DebugViewerBase
{
    protected override VisualElement CreateViewGUI()
    {
        var root = base.CreateViewGUI();
        var historyTextField = new HistoryTextField("Input");
        historyTextField.RegisterValueChangedCallback(evt => Debug.Log(evt.newValue));
        root.Add(historyTextField);
        return root;
    }
}
#endif
```

## Theming

The look of DebugToolkit is defined by the USS under `ExternalResources/`. The base colors are
low-saturation (close to neutral gray) and the surfaces are translucent, so the menu stays readable
over any game background while still letting you see the game underneath.

### Layout

The sheets are split by responsibility and pulled together by `DebugToolkitUss.uss` with `@import`.
The import order matters: rules of equal specificity are resolved last-one-wins.

| File | Contents |
| --- | --- |
| `Parts/Variables.uss` | Color and metric tokens. Every other sheet reads these and nothing else |
| `Parts/Base.uss` | The window shell, Label, BaseField — the foundation |
| `Parts/Buttons.uss` | Button / ButtonGroup / ToggleButtonGroup |
| `Parts/Fields.uss` | TextField / Toggle / RadioButton / Popup / Enum / Dropdown / Vector fields / Bounds |
| `Parts/Sliders.uss` | Slider / MinMaxSlider / Scroller / ScrollView / ProgressBar |
| `Parts/Containers.uss` | Foldout / Box / HelpBox / GroupBox / TabView / TwoPaneSplitView |
| `Parts/Collections.uss` | ListView / TreeView / MultiColumnListView |
| `Parts/Window.uss` | Debug windows, headers, console — the DebugToolkit-specific elements |

### State conventions

Every interactive element shows its state the same way, through the same tokens.

| State | How it is shown |
| --- | --- |
| `:hover` | One step lighter (`*-hover`) |
| `:active` | One step darker (`*-active`) |
| `:focus` | Border color becomes `--debug-toolkit-color-focus`; the border width never changes, so nothing shifts |
| `:checked` | `--debug-toolkit-color-selected` (success / danger for the on/off of a Toggle) |
| `:disabled` | `*-disabled` plus `--debug-toolkit-color-text-disabled` |

### Recoloring

Override the custom properties on `:root` from your own style sheet — no need to fork the package.

```css
:root {
    /* Let more of the game through. */
    --debug-toolkit-color-surface: rgba(20, 22, 25, 0.5);
    /* Match your project's accent. */
    --debug-toolkit-color-accent: rgb(200, 140, 60);
}
```

The main tokens are below; see `Parts/Variables.uss` for the full set.

| Token | Used for |
| --- | --- |
| `--debug-toolkit-neutral-00`–`-10` | The low-saturation base ramp, darkest to lightest |
| `--debug-toolkit-color-surface` | The window itself (translucent) |
| `--debug-toolkit-color-surface-raised` / `-sunken` | Sections / inputs and grooves |
| `--debug-toolkit-color-surface-overlay` | Surfaces that must stay opaque, such as dropdowns |
| `--debug-toolkit-color-control` / `-hover` / `-active` / `-disabled` | Interactive controls |
| `--debug-toolkit-color-accent` / `-hover` / `-active` / `-disabled` | Draggers, progress fills |
| `--debug-toolkit-color-selected` / `-hover` | Selection |
| `--debug-toolkit-color-focus` | Focus border |
| `--debug-toolkit-color-success` / `-danger` / `-warning` | On/off and log severity |
| `--debug-toolkit-color-text` / `-dim` / `-disabled` | Text |

The few colors that depend on runtime state and therefore come from C# (log row backgrounds, the
window-list toggles) live in `DebugConst.StyleColor`. They mirror the USS tokens: change one and the
other has to follow.

## Requirements

- Unity 2021.3 or newer
- UIElements (UIToolkit)

## License

This library is under the MIT License.