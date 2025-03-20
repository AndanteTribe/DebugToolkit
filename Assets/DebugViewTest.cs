#if ENABLE_DEBUGTOOLKIT
using UnityEngine;
using UnityEngine.UIElements;
using DebugToolkit;

public class DebugViewTest : DebugViewerBase
{
    protected override VisualElement Setup()
    {
        var root = base.Setup();
        var tabView = root.AddTabView();
        var tab1 = tabView.AddTab("Tab1");
        for (int i = 0; i < 5; i++)
        {
            tab1.Add(new Button(){text = $"Button{i + 1}"});
        }
        
        var tab2 = tabView.AddTab("Tab2");
        tab2.AddProfileInfoLabel();
        return root;
    }
}
#endif