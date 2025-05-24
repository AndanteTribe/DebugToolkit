using UnityEngine.UIElements;

namespace DebugToolkit.Tests
{
    public class DebugViewWindowTest:DebugViewerBase
    {
        public VisualElement Root { get; private set; }

        protected override VisualElement CreateViewGUI()
        {
            var root = base.CreateViewGUI();
            Root = root;
            var window0 = root.AddWindow("TestWindow1");
            window0.Add(new Button(){text = "TestButton"});
            var window1 = root.AddWindow("TestWindow2");
            window1.AddProfileInfoLabel();
            return root;
        }
    }
}