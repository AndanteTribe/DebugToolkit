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
        public　override async Task SetUp()
        {
            await base.SetUp();
            _debugViewWindowTest = new DebugViewWindowTest();
            _debugViewWindowTest.Start();
        }

        [TearDown]
        public override async Task TearDown()
        {
            await base.TearDown();

            //  インスタンスの破棄、場合によってはやめた方がいいかも？
            _debugViewWindowTest = null;
            DebugViewerBase.MasterWindow = null;
            DebugViewerBase.DebugWindowList.Clear();
        }

        // マスターウィンドウが正しく生成されるかテスト
        //  いずれマスターウィンドウをstaticじゃなくするときは変更する
        [Test]
        public void MasterWindow_IsCorrectlyGenerated()
        {
            Assert.That(DebugViewerBase.MasterWindow, Is.Not.Null, "MasterWindow should be generated.");
            Assert.That(DebugViewerBase.MasterWindow.parent.Q<Label>(
                    name: "window-label",
                    className: DebugConst.WindowLabelClassName)
                .text, Is.EqualTo("Debug Toolkit"), "MasterWindow title is incorrect.");
        }

        // マスターウィンドウに他のウィンドウを表示するボタンが生成されているかテスト
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

        // マスターウィンドウのウィンドウ表示ボタンが正しく動作するかテスト
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
            Input.Set(mouse.position, new Vector2(screenPosX, screenPosY));
            Input.Click(mouse.leftButton);
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            Assert.That(testWindow.style.display.value, Is.EqualTo(DisplayStyle.Flex),
                "Window should be visible after toggle.");
            Assert.That(toggle.style.backgroundColor.value, Is.EqualTo(new Color(0.4f, 0.8f, 0.4f)),
                "Toggle color for visible window is incorrect.");

            Input.Set(mouse.position, new Vector2(screenPosX, screenPosY));
            Input.Click(mouse.leftButton);
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();


            Assert.That(testWindow.style.display.value, Is.EqualTo(DisplayStyle.None),
                "Window should be hidden after toggle.");
            Assert.That(toggle.style.backgroundColor.value, Is.EqualTo(new Color(0.6f, 0.2f, 0.2f)),
                "Toggle color for hidden window is incorrect.");
        }

        // マスターウィンドウを最小化できるかテスト
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
            Input.Set(mouse.position, new Vector2(422, 995));
            Input.Click(mouse.leftButton);
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            Assert.That(windowContent.style.display.value, Is.EqualTo(DisplayStyle.None),
                "Window content should be hidden after minimizing.");
            Assert.That(minimizeButton.text, Is.EqualTo("^"),
                "Minimize button text should change to '^' when minimized.");

            Input.Click(mouse.leftButton);
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            Assert.That(windowContent.style.display.value, Is.EqualTo(DisplayStyle.Flex),
                "Window content should be visible after maximizing.");
            Assert.That(minimizeButton.text, Is.EqualTo("-"),
                "Minimize button text should change to '-' when maximized.");
        }

        // マスターウィンドウが１つしかないかテスト
        [Test]
        public void MasterWindow_OnlyOneExists()
        {
            var masterWindows = _debugViewWindowTest.Root
                .Query<VisualElement>(className: DebugConst.ClassName + "__master-window").ToList();
            Assert.That(masterWindows.Count, Is.EqualTo(1), "There should be exactly one master window.");
        }

        // 全表示非表示ボタンが表示されているかテスト
        [Test]
        public void ToggleAllButton_IsVisible()
        {
            var toggleAllButton =
                _debugViewWindowTest.Root.Q<Button>(className: DebugConst.ClassName + "__toggle-all-button");
            Assert.That(toggleAllButton, Is.Not.Null, "Toggle all button should be visible.");
        }

        // 全表示非表示ボタンが正しく機能するかテスト
        [Test]
        public async Task ToggleAllButton_TogglesAllWindowsVisibility()
        {
            foreach (var window in DebugViewerBase.DebugWindowList)
            {
                window.style.display = DisplayStyle.Flex;
            }

            var mouse = InputSystem.AddDevice<Mouse>();
            Input.Set(mouse.position, new Vector2(15, 20));
            Input.Click(mouse.leftButton);
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            foreach (var window in DebugViewerBase.DebugWindowList)
            {
                Assert.That(window.style.display.value, Is.EqualTo(DisplayStyle.None),
                    "Window should be hidden after toggle all.");
            }

            Input.Click(mouse.leftButton);
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            foreach (var window in DebugViewerBase.DebugWindowList)
            {
                Assert.That(window.style.display.value, Is.Not.EqualTo(DisplayStyle.None),
                    "Window should be visible after toggling back.");
            }
        }

        // 全表示非表示ボタンが１つしかないかテスト
        [Test]
        public void ToggleAllButton_OnlyOneExists()
        {
            var toggleAllButtons = _debugViewWindowTest.Root
                .Query<Button>(className: DebugConst.ClassName + "__toggle-all-button").ToList();
            Assert.That(toggleAllButtons.Count, Is.EqualTo(1), "There should be exactly one toggle all button.");
        }

        // AddWindowで正しいrootが帰って来るかテスト
        [Test]
        public void AddWindow_ReturnsCorrectRoot()
        {
            var newWindowContent = _debugViewWindowTest.Root.AddWindow("TestNewWindow");
            Assert.That(newWindowContent, Is.Not.Null, "AddWindow should return a valid VisualElement.");
            Assert.That(newWindowContent.ClassListContains(DebugConst.WindowContentClassName), Is.True,
                "Returned element should have the window content class.");
        }

        // AddWindowの引数でとったwindowNameが正しく表示されてるかテスト
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

        // ウィンドウがクリックされたら、最前面に来るかテスト
        [Test]
        public async Task Window_BringsToFrontWhenClicked()
        {
            var windows = DebugViewerBase.DebugWindowList;
            foreach (var window in windows)
            {
                window.style.display = DisplayStyle.Flex;
            }

            var mouse = InputSystem.AddDevice<Mouse>();
            Input.Set(mouse.position, new Vector2(476, 894));
            Input.Click(mouse.leftButton);
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            var parent = windows[0].parent;
            int lastIndex = parent.childCount - 1;
            Assert.That(parent[lastIndex], Is.EqualTo(windows[1]),
                "Clicked window1 should be brought to front (last child in parent).");

            Input.Set(mouse.position, new Vector2(557, 828));
            Input.Click(mouse.leftButton);
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            Assert.That(parent[lastIndex], Is.EqualTo(windows[2]),
                "Clicked window2 should be brought to front (last child in parent).");
        }

        // ウィンドウが正しく追加できているかテスト
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

        // ウィンドウヘッダーが正しく追加されているかテスト
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

        // ウィンドウの✕ボタンで非表示にできるか & マスターウィンドウ上で表示ボタンの状態が正しく変わるかテスト
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
            var closeButton = window.Q<Button>(className: DebugConst.ClassName + "__delete-button");

            var mouse = InputSystem.AddDevice<Mouse>();
            Input.Set(mouse.position, new Vector2(screenPosX, screenPosY));
            Input.Click(mouse.leftButton);
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            Assert.That(window.style.display.value, Is.EqualTo(DisplayStyle.Flex),
                "Window should be visible initially.");
            Assert.That(toggle.style.backgroundColor.value, Is.EqualTo(new Color(0.4f, 0.8f, 0.4f)),
                "Toggle color should indicate visible window.");

            Input.Set(mouse.position, new Vector2(screenPosX, screenPosY));
            Input.Click(mouse.leftButton);
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            Assert.That(window.style.display.value, Is.EqualTo(DisplayStyle.None),
                "Window should be hidden after clicking close button.");
            Assert.That(toggle.style.backgroundColor.value, Is.EqualTo(new Color(0.6f, 0.2f, 0.2f)),
                "Toggle color should indicate hidden window.");
        }
    }
}