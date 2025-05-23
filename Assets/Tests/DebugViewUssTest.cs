using UnityEngine.UIElements;

namespace DebugToolkit.Tests
{
    public class DebugViewUssTest : DebugViewerBase
    {
        public VisualElement Root { get; private set; }

        protected override VisualElement CreateViewGUI()
        {
            var root = base.CreateViewGUI();
            Root = root;
            return root;
        }
    }
}