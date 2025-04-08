#if ENABLE_DEBUGTOOLKIT
using UnityEngine.UIElements;
using DebugToolkit;

public class DebugViewTest : DebugViewerBase
{
    protected override VisualElement CreateViewGUI()
    {
        var root = base.CreateViewGUI();
        var (tabRoot, tab1) = root.AddTab();
        var tab2 = tabRoot.AddTab("Tab2");
        
        for (int i = 0; i < 5; i++)
        {
            tab1.Add(new Button(){text = $"Button{i + 1}"});
        }
        tab2.AddProfileInfoLabel();
        return root;
    }
}
#endif