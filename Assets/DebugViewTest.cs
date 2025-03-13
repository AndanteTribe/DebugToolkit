#if ENABLE_DEBUGTOOLKIT
using UnityEngine;
using UnityEngine.UIElements;
using DebugToolkit;

public class DebugViewTest : DebugViewerBase
{
    protected override VisualElement Setup()
    {
        var root = base.Setup();
        var tab1 = AddTab("Tab1");
        tab1.Add(new Label(){text = "Hello, World!"});
        for (int i = 0; i < 5; i++)
        {
            tab1.Add(new Button(){text = $"Button{i + 1}"});
        }
        
        var tab2 = AddTab("Tab2");
        tab2.Add(new Label(){text = "This is New Tab!!"});
        return root;
    }
}
#endif