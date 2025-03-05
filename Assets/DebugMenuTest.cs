using UnityEngine;
using UnityEngine.UIElements;
using DebugToolkit;
public class DebugMenuTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var debugViewer = new DefaultDebugView();
        var root = debugViewer.Start();

        Tab tab = root.parent as Tab;
        tab.label = "Normal";
        
        root.Add(new Label("Hello World!"));
        for(int i = 0; i < 5; i++)
        {
            root.Add(new Button() { text = "Button" + (i + 1)});
        }

        Tab tab2 = new Tab { label = "Warning" };
        var root2 = new ScrollView();
        tab2.Add(root2);
        root.parent.parent.Add(tab2);
    }
    
    void Update()
    {
        
    }
}
