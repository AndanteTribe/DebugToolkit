#if ENABLE_DEBUGTOOLKIT
using UnityEngine.UIElements;
using DebugToolkit;
using UnityEngine;

public class DebugViewTest : DebugViewerBase
{
    protected override VisualElement CreateViewGUI()
    {
        var root = base.CreateViewGUI();
        var window = root.AddWindow("TestWindow");
        var (tabRoot, tab1) = window.AddTab();
        var tab2 = tabRoot.AddTab("Tab2");

        var testButton = new Button(){text = "Hoge"};
        testButton.clicked += () => Debug.Log("TestButton");
        tab1.Add(testButton);
        var logButton = new Button(){ text = "Log" };
        logButton.clicked += () => Debug.Log("Log test message");
        tab1.Add(logButton);

        var warningButton = new Button(){ text = "Warning" };
        warningButton.clicked += () => Debug.LogWarning("Warning test message");
        tab1.Add(warningButton);

        var errorButton = new Button(){ text = "Error" };
        errorButton.clicked += () => Debug.LogError("Error test message");
        tab1.Add(errorButton);

        tab2.AddProfileInfoLabel();

        var window2 = root.AddWindow("TestWindow2");
        window2.AddConsoleView();
        return root;
    }
}
#endif