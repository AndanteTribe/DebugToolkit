using UnityEngine;

namespace DebugToolkit
{
    /// <summary>
    /// Simple MonoBehaviour to test the debug export functionality in a scene.
    /// Add this to a GameObject and call StartDemo() to see the export feature in action.
    /// </summary>
    public class DebugExportTestRunner : MonoBehaviour
    {
        [Header("Debug Export Test")]
        [Tooltip("Click the button in the Inspector or call StartDemo() to test export functionality")]
        public bool runDemo = false;

        private DebugExportDemo demoViewer;

        void Start()
        {
            if (runDemo)
            {
                StartDemo();
            }
        }

        void Update()
        {
            if (runDemo)
            {
                runDemo = false;
                StartDemo();
            }
        }

        [ContextMenu("Start Export Demo")]
        public void StartDemo()
        {
            if (demoViewer == null)
            {
                demoViewer = new DebugExportDemo();
                demoViewer.Start();
                
                Debug.Log("Debug Export Demo started! Look for the 'Export Demo' window in the UI.");
                Debug.Log("Adjust values and click JSON/XML buttons to test export functionality.");
            }
            else
            {
                Debug.Log("Demo is already running!");
            }
        }

        [ContextMenu("Test Direct JSON Export")]
        public void TestJsonExport()
        {
            if (demoViewer?.MasterWindow != null)
            {
                DebugExportUtils.ExportToJson(demoViewer.MasterWindow);
                Debug.Log("JSON export completed! Check your clipboard.");
            }
            else
            {
                Debug.LogWarning("Demo not running. Call StartDemo() first.");
            }
        }

        [ContextMenu("Test Direct XML Export")]
        public void TestXmlExport()
        {
            if (demoViewer?.MasterWindow != null)
            {
                DebugExportUtils.ExportToXml(demoViewer.MasterWindow);
                Debug.Log("XML export completed! Check your clipboard.");
            }
            else
            {
                Debug.LogWarning("Demo not running. Call StartDemo() first.");
            }
        }

        void OnDestroy()
        {
            // Clean up when the component is destroyed
            if (demoViewer?.MasterWindow?.panel != null)
            {
                DestroyImmediate(demoViewer.MasterWindow.panel.GetRootVisualContainer().gameObject);
            }
        }
    }
}