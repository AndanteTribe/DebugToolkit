using UnityEngine;
using UnityEngine.UIElements;

namespace DebugToolkit
{
    public class DefaultDebugView : DebugViewerBase
    {
        private bool _isDragging = false;
            public override VisualElement Start()
            {
                var safeAreaContainer = base.Start();
                
                var styleSheet = Resources.Load<StyleSheet>("DebugToolkitUss");
                
                var uiDocument = Object.FindObjectOfType<UIDocument>();
                if (uiDocument != null)
                {
                    var root = uiDocument.rootVisualElement;
                    root.styleSheets.Add(styleSheet);
                }
                
                var windowMaster = new VisualElement();
                windowMaster.AddToClassList("debug-toolkit-master");
                safeAreaContainer.Add(windowMaster);
                
                var window = new VisualElement();
                window.AddToClassList("debug-window");
                windowMaster.Add(window);
                
                var foldout = new Foldout { text = "DebugMenu", value = true, name = "DebugMenuHandler"};
                window.Add(foldout);
                
                var tabView = new TabView();
                foldout.Add(tabView);
                
                var tab = new Tab { label = "Tab1" };
                tabView.Add(tab);
                
                var scrollView = new ScrollView();
                tab.Add(scrollView);
                
                return scrollView;
            }
    }
}