using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using System.Linq;

namespace DebugToolkit.Tests
{
    public class WindowTest : TestBase
    {
        private DebugViewWindowTest _debugViewWindowTest;

        [OneTimeSetUp]
        public override void OneTimeSetUp() => base.OneTimeSetUp();

        [SetUp]
        public override async Task SetUp()
        {
            await base.SetUp();
            _debugViewWindowTest = new DebugViewWindowTest();
            _debugViewWindowTest.Start();
        }

        [TearDown]
        public override async Task TearDown()
        {
            await base.TearDown();

            //  Disposing instances, might be better to reconsider this approach?
            _debugViewWindowTest = null;
        }

        // Test if the master window is correctly generated
        //  This will need to be changed when the master window is no longer static
        [Test]
        public void MasterWindow_IsCorrectlyGenerated()
        {
            Assert.That(DebugViewerBase.MasterWindow, Is.Not.Null, "MasterWindow should be generated.");
            Assert.That(DebugViewerBase.MasterWindow.parent.Q<Label>(
                    name: "window-label",
                    className: DebugConst.WindowLabelClassName)
                .text, Is.EqualTo("Debug Toolkit"), "MasterWindow title is incorrect.");
        }

        // Test if buttons to display other windows are generated in the master window
        [Test]
        public void MasterWindow_ContainsWindowListButtonsForOtherWindows()
        {
            var windowListScrollView =
                DebugViewerBase.MasterWindow.Q<ScrollView>(className: DebugConst.WindowListClassName);
            Assert.That(windowListScrollView, Is.Not.Null, "WindowList ScrollView not found in MasterWindow.");

            var toggles = windowListScrollView.Query<Toggle>(className: DebugConst.ToggleWindowDisplayClassName)
                .ToList();
            Assert.That(toggles.Count, Is.EqualTo(2), "Incorrect number of window display toggles in MasterWindow.");
            Assert.That(toggles.Any(t => t.text == "TestWindow1"), Is.True, "Toggle for TestWindow1 not found.");
            Assert.That(toggles.Any(t => t.text == "TestWindow2"), Is.True, "Toggle for TestWindow2 not found.");
        }

        // Test if the window display buttons in the master window work correctly
        [Test]
        [TestCase("TestWindow1", 110, 930)]
        [TestCase("TestWindow2", 112, 872)]
        public async Task MasterWindow_WindowListButton_TogglesWindowVisibility(string windowName, float screenPosX,
            float screenPosY)
        {
            var toggle = DebugViewerBase.MasterWindow.Q<ScrollView>(className: DebugConst.WindowListClassName)
                .Query<Toggle>(className: DebugConst.ToggleWindowDisplayClassName)
                .Where(t => t.text == windowName).First();

            var testWindow = _debugViewWindowTest.Root.Q<VisualElement>(name: windowName);

            Assert.That(testWindow.style.display.value, Is.EqualTo(DisplayStyle.None),
                "Window should be visible first time.");
            Assert.That(toggle.style.backgroundColor.value, Is.EqualTo(new Color(0.6f, 0.2f, 0.2f)),
                "Toggle color for hidden window is incorrect.");


            var mouse = InputSystem.AddDevice<Mouse>();
            await ClickAtPositionAsync(mouse, new Vector2(screenPosX, screenPosY));

            Assert.That(testWindow.style.display.value, Is.EqualTo(DisplayStyle.Flex),
                "Window should be visible after toggle.");
            Assert.That(toggle.style.backgroundColor.value, Is.EqualTo(new Color(0.4f, 0.8f, 0.4f)),
                "Toggle color for visible window is incorrect.");

            await ClickAtPositionAsync(mouse,  new Vector2(screenPosX, screenPosY));

            Assert.That(testWindow.style.display.value, Is.EqualTo(DisplayStyle.None),
                "Window should be hidden after toggle.");
            Assert.That(toggle.style.backgroundColor.value, Is.EqualTo(new Color(0.6f, 0.2f, 0.2f)),
                "Toggle color for hidden window is incorrect.");
        }

        // Test if the master window can be minimized
        [Test]
        public async Task MasterWindow_CanBeMinimized()
        {
            var masterWindow = DebugViewerBase.MasterWindow.parent;
            var minimizeButton = masterWindow.Q<Button>(className: DebugConst.ClassName + "__minimize-button");
            var windowContent = DebugViewerBase.MasterWindow;

            Assert.That(minimizeButton, Is.Not.Null, "Minimize button not found in MasterWindow.");
            Assert.That(windowContent.style.display.value, Is.EqualTo(DisplayStyle.Flex),
                "Window content should be visible initially.");

            var mouse = InputSystem.AddDevice<Mouse>();
            await ClickAtPositionAsync(mouse,  new Vector2(422, 995));

            Assert.That(windowContent.style.display.value, Is.EqualTo(DisplayStyle.None),
                "Window content should be hidden after minimizing.");
            Assert.That(minimizeButton.text, Is.EqualTo("^"),
                "Minimize button text should change to '^' when minimized.");

            await ClickAtPositionAsync(mouse,  new Vector2(422, 995));

            Assert.That(windowContent.style.display.value, Is.EqualTo(DisplayStyle.Flex),
                "Window content should be visible after maximizing.");
            Assert.That(minimizeButton.text, Is.EqualTo("-"),
                "Minimize button text should change to '-' when maximized.");
        }

        // Test if there is only one master window
        [Test]
        public void MasterWindow_OnlyOneExists()
        {
            var masterWindows = _debugViewWindowTest.Root
                .Query<VisualElement>(className: DebugConst.ClassName + "__master-window").ToList();
            Assert.That(masterWindows.Count, Is.EqualTo(1), "There should be exactly one master window.");
        }

        // Test if the toggle all button is visible
        [Test]
        public void ToggleAllButton_IsVisible()
        {
            var toggleAllButton =
                _debugViewWindowTest.Root.Q<Button>(className: DebugConst.ClassName + "__toggle-all-button");
            Assert.That(toggleAllButton, Is.Not.Null, "Toggle all button should be visible.");
        }

        // Test if the toggle all button functions correctly
        [Test]
        public async Task ToggleAllButton_TogglesAllWindowsVisibility()
        {
            foreach (var window in DebugViewerBase.DebugWindowList)
            {
                window.style.display = DisplayStyle.Flex;
            }

            var mouse = InputSystem.AddDevice<Mouse>();
            await ClickAtPositionAsync(mouse, new Vector2(15, 20));

            foreach (var window in DebugViewerBase.DebugWindowList)
            {
                Assert.That(window.style.display.value, Is.EqualTo(DisplayStyle.None),
                    "Window should be hidden after toggle all.");
            }

            await ClickAtPositionAsync(mouse, new Vector2(15, 20));

            foreach (var window in DebugViewerBase.DebugWindowList)
            {
                Assert.That(window.style.display.value, Is.Not.EqualTo(DisplayStyle.None),
                    "Window should be visible after toggling back.");
            }
        }

        // Test if there is only one toggle all button
        [Test]
        public void ToggleAllButton_OnlyOneExists()
        {
            var toggleAllButtons = _debugViewWindowTest.Root
                .Query<Button>(className: DebugConst.ClassName + "__toggle-all-button").ToList();
            Assert.That(toggleAllButtons.Count, Is.EqualTo(1), "There should be exactly one toggle all button.");
        }

        // Test if AddWindow returns the correct root
        [Test]
        public void AddWindow_ReturnsCorrectRoot()
        {
            var newWindowContent = _debugViewWindowTest.Root.AddWindow("TestNewWindow");
            Assert.That(newWindowContent, Is.Not.Null, "AddWindow should return a valid VisualElement.");
            Assert.That(newWindowContent.ClassListContains(DebugConst.WindowContentClassName), Is.True,
                "Returned element should have the window content class.");
        }

        // Test if the windowName taken as an argument in AddWindow is displayed correctly
        [Test]
        public void AddWindow_DisplaysCorrectWindowName()
        {
            var windowName = "UniqueWindowName";
            var windowContent = _debugViewWindowTest.Root.AddWindow(windowName);
            var window = windowContent.parent;
            var windowLabel = window.Q<Label>(className: DebugConst.WindowLabelClassName);

            Assert.That(windowLabel, Is.Not.Null, "Window label not found.");
            Assert.That(windowLabel.text, Is.EqualTo(windowName), "Window name is not displayed correctly.");
        }

        // Test if the window comes to the front when clicked
        [Test]
        public async Task Window_BringsToFrontWhenClicked()
        {
            var windows = DebugViewerBase.DebugWindowList;
            foreach (var window in windows)
            {
                window.style.display = DisplayStyle.Flex;
            }

            var mouse = InputSystem.AddDevice<Mouse>();
            await ClickAtPositionAsync(mouse, new Vector2(476, 894));

            var parent = windows[0].parent;
            int lastIndex = parent.childCount - 1;
            Assert.That(parent[lastIndex], Is.EqualTo(windows[1]),
                "Clicked window1 should be brought to front (last child in parent).");

            await ClickAtPositionAsync(mouse, new Vector2(557, 828));

            Assert.That(parent[lastIndex], Is.EqualTo(windows[2]),
                "Clicked window2 should be brought to front (last child in parent).");
        }

        // Test if the window is added correctly
        [Test]
        public void Window_IsCorrectlyAdded()
        {
            var windowContent = _debugViewWindowTest.Root.AddWindow("TestAddedWindow");
            var window = windowContent.parent;

            Assert.That(window, Is.Not.Null, "Window should be added to the DOM.");
            Assert.That(window.ClassListContains(DebugConst.ClassName + "__normal-window"), Is.True,
                "Added window should have the normal window class.");
            Assert.That(DebugViewerBase.DebugWindowList.Contains(window), Is.True,
                "Window should be added to the debug window list.");
        }

        // Test if the window header is added correctly
        [Test]
        public void WindowHeader_IsCorrectlyAdded()
        {
            var windowContent = _debugViewWindowTest.Root.AddWindow("TestHeaderWindow");
            var window = windowContent.parent;
            var header = window.Q<VisualElement>(className: DebugConst.WindowHeaderClassName);

            Assert.That(header, Is.Not.Null, "Window header should be added.");
            Assert.That(header.Q<Label>(className: DebugConst.WindowLabelClassName), Is.Not.Null,
                "Window header should contain a label.");
            Assert.That(header.Q<VisualElement>(name: "drag-area"), Is.Not.Null,
                "Window header should contain a drag area.");
            Assert.That(header.Q<Button>(className: DebugConst.ClassName + "__delete-button"), Is.Not.Null,
                "Normal window header should contain a delete button.");
        }

        // Test if the window can be hidden using the X button & if the display button state changes correctly in the master window
        [Test]
        [TestCase("TestWindow1", 110, 930)]
        [TestCase("TestWindow2", 112, 872)]
        public async Task WindowCloseButton_HidesWindowAndUpdatesToggle(string windowName, float screenPosX,
            float screenPosY)
        {
            var toggle = DebugViewerBase.MasterWindow.Q<ScrollView>(className: DebugConst.WindowListClassName)
                .Query<Toggle>(className: DebugConst.ToggleWindowDisplayClassName)
                .Where(t => t.text == windowName).First();

            var window = _debugViewWindowTest.Root.Q<VisualElement>(name: windowName);

            var mouse = InputSystem.AddDevice<Mouse>();
            await ClickAtPositionAsync(mouse, new Vector2(screenPosX, screenPosY));

            Assert.That(window.style.display.value, Is.EqualTo(DisplayStyle.Flex),
                "Window should be visible initially.");
            Assert.That(toggle.style.backgroundColor.value, Is.EqualTo(new Color(0.4f, 0.8f, 0.4f)),
                "Toggle color should indicate visible window.");

            await ClickAtPositionAsync(mouse,new Vector2(screenPosX, screenPosY));

            Assert.That(window.style.display.value, Is.EqualTo(DisplayStyle.None),
                "Window should be hidden after clicking close button.");
            Assert.That(toggle.style.backgroundColor.value, Is.EqualTo(new Color(0.6f, 0.2f, 0.2f)),
                "Toggle color should indicate hidden window.");
        }
    }
}
