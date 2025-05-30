using UnityEngine.UIElements;

namespace DebugToolkit.Tests
{
    public class DebugViewTabTest : DebugViewerBase
    {
        public VisualElement Root { get; private set; }

        protected override VisualElement CreateViewGUI()
        {
            var root = base.CreateViewGUI();
            Root = root;
            var window0 = root.AddWindow("TestWindow1");
            var (tabRoot, tab1 ) = window0.AddTab("Tab1");
            tab1.Add(new Label(){text = "TestTab1"});
            tab1.Add(new Button(){text = "TestButton1"});
            var tab2 = tabRoot.AddTab("Tab2");
            tab2.AddProfileInfoLabel();
            return root;
        }
    }
}