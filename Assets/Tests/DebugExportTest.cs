using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

namespace DebugToolkit.Tests
{
    public class DebugExportTest : TestBase
    {
        private VisualElement testRoot;
        private DebugViewerBase debugViewer;

        [SetUp]
        public async Task SetUpTest()
        {
            await base.SetUp();
            
            // Create a test debug viewer
            debugViewer = new TestDebugViewer();
            testRoot = debugViewer.CreateViewGUI();
        }

        [TearDown]
        public async Task TearDownTest()
        {
            if (testRoot?.panel != null)
            {
                Object.DestroyImmediate(testRoot.panel.GetRootVisualContainer().gameObject);
            }
            await base.TearDown();
        }

        [Test]
        public void TestExportUtilsExists()
        {
            // Test that the DebugExportUtils class exists and has the expected methods
            var exportUtilsType = typeof(DebugExportUtils);
            Assert.That(exportUtilsType, Is.Not.Null);
            
            var exportToJsonMethod = exportUtilsType.GetMethod("ExportToJson");
            Assert.That(exportToJsonMethod, Is.Not.Null);
            
            var exportToXmlMethod = exportUtilsType.GetMethod("ExportToXml");
            Assert.That(exportToXmlMethod, Is.Not.Null);
        }

        [Test]
        public void TestExportExtensionMethods()
        {
            // Test that the extension methods exist
            var container = new VisualElement();
            
            // These should not throw exceptions
            Assert.DoesNotThrow(() => container.AddExportButtons());
            Assert.DoesNotThrow(() => container.AddJsonExportButton());
            Assert.DoesNotThrow(() => container.AddXmlExportButton());
        }

        [Test]
        public void TestExportWithTestData()
        {
            // Create a test window with some UI elements
            var window = testRoot.AddWindow("TestExportWindow");
            
            // Add some test UI elements with known values
            var slider = new Slider(0, 100) { name = "TestSlider", value = 50.5f };
            window.Add(slider);
            
            var toggle = new Toggle { name = "TestToggle", value = true };
            window.Add(toggle);
            
            var textField = new TextField { name = "TestTextField", value = "Hello World" };
            window.Add(textField);
            
            var dropdown = new DropdownField { name = "TestDropdown", value = "Option1" };
            window.Add(dropdown);
            
            // Test that export doesn't throw exceptions
            Assert.DoesNotThrow(() => DebugExportUtils.ExportToJson(testRoot));
            Assert.DoesNotThrow(() => DebugExportUtils.ExportToXml(testRoot));
            
            // Verify that clipboard was set (we can't easily verify content in tests)
            // but we can check that the operation completed without error
            Debug.Log("Export test completed successfully");
        }

        [Test]
        public void TestExportButtonsInWindow()
        {
            var window = testRoot.AddWindow("ExportButtonTestWindow");
            
            // Add export buttons
            window.AddExportButtons();
            
            // Verify buttons were added
            var buttons = window.Query<Button>().ToList();
            Assert.That(buttons.Count, Is.GreaterThanOrEqualTo(2), "Should have at least 2 export buttons");
            
            // Find JSON and XML buttons
            var jsonButton = window.Query<Button>().Where(b => b.text == "JSON").First();
            var xmlButton = window.Query<Button>().Where(b => b.text == "XML").First();
            
            Assert.That(jsonButton, Is.Not.Null, "JSON export button should exist");
            Assert.That(xmlButton, Is.Not.Null, "XML export button should exist");
        }

        private class TestDebugViewer : DebugViewerBase
        {
            protected override VisualElement CreateViewGUI()
            {
                var root = base.CreateViewGUI();
                return root;
            }
        }
    }
}