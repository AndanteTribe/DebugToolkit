using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Task = System.Threading.Tasks.Task;
using System.Linq;

namespace DebugToolkit.Tests
{
    public class WindowTest
    {
        private DebugViewWindowTest _debugViewWindowTest;
        private readonly InputTestFixture _input = new();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var gameView = GetGameView();
            gameView.minSize = new Vector2(1920, 1080);
            gameView.position = new Rect(0, 0, 1920, 1080);
        }

        [SetUp]
        public async Task SetUp()
        {
            var document = Object.FindAnyObjectByType<UIDocument>();
            if (document != null)
            {
                Object.DestroyImmediate(document.gameObject);
            }

            _input.Setup();
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            await SceneManager.LoadSceneAsync("DefaultTests", LoadSceneMode.Additive);
            _debugViewWindowTest = new DebugViewWindowTest();
            _debugViewWindowTest.Start();
            await Awaitable.NextFrameAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            _input.TearDown();
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.ResetAndDisableNonBackgroundDevices;
            var testScene = SceneManager.GetSceneByName("DefaultTests");
            if (testScene.isLoaded)
            {
                await SceneManager.UnloadSceneAsync(testScene);
            }

            //  インスタンスの破棄、場合によってはやめた方がいいかも？
            _debugViewWindowTest = null;
            DebugViewerBase.MasterWindow = null;
            DebugViewerBase.DebugWindowList.Clear();
        }

        [Test]
        public void InputNullTest()
        {
            Assert.That(_input, Is.Not.Null);
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
            var windowListScrollView = DebugViewerBase.MasterWindow.Q<ScrollView>(className: DebugConst.WindowListClassName);
            Assert.That(windowListScrollView, Is.Not.Null, "WindowList ScrollView not found in MasterWindow.");

            var toggles = windowListScrollView.Query<Toggle>(className: DebugConst.ToggleWindowDisplayClassName).ToList();
            Assert.That(toggles.Count, Is.EqualTo(2), "Incorrect number of window display toggles in MasterWindow.");
            Assert.That(toggles.Any(t => t.text == "TestWindow1"), Is.True, "Toggle for TestWindow1 not found.");
            Assert.That(toggles.Any(t => t.text == "TestWindow2"), Is.True, "Toggle for TestWindow2 not found.");
        }

        // マスターウィンドウのウィンドウ表示ボタンが正しく動作するかテスト
        [Test]
        [TestCase("TestWindow1",110, 930)]
        [TestCase("TestWindow2",112, 872)]
        public async Task MasterWindow_WindowListButton_TogglesWindowVisibility(string windowName, float screenPosX, float screenPosY)
        {
            var toggle = DebugViewerBase.MasterWindow.Q<ScrollView>(className: DebugConst.WindowListClassName)
                .Query<Toggle>(className: DebugConst.ToggleWindowDisplayClassName)
                .Where(t => t.text == windowName).First();

            var testWindow = _debugViewWindowTest.Root.Q<VisualElement>(name: windowName);

            Assert.That(testWindow.style.display.value, Is.EqualTo(DisplayStyle.None), "Window should be visible first time.");
            Assert.That(toggle.style.backgroundColor.value, Is.EqualTo(new Color(0.6f, 0.2f, 0.2f)), "Toggle color for hidden window is incorrect.");


            var mouse = InputSystem.AddDevice<Mouse>();
            _input.Set(mouse.position, new Vector2(screenPosX, screenPosY));
            _input.Click(mouse.leftButton);
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            Assert.That(testWindow.style.display.value, Is.EqualTo(DisplayStyle.Flex), "Window should be visible after toggle.");
            Assert.That(toggle.style.backgroundColor.value, Is.EqualTo(new Color(0.4f, 0.8f, 0.4f)), "Toggle color for visible window is incorrect.");

            _input.Set(mouse.position, new Vector2(screenPosX, screenPosY));
            _input.Click(mouse.leftButton);
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();


            Assert.That(testWindow.style.display.value, Is.EqualTo(DisplayStyle.None), "Window should be hidden after toggle.");
            Assert.That(toggle.style.backgroundColor.value, Is.EqualTo(new Color(0.6f, 0.2f, 0.2f)), "Toggle color for hidden window is incorrect.");
        }

        private static EditorWindow GetGameView()
            => EditorWindow.GetWindow(System.Type.GetType("UnityEditor.GameView,UnityEditor"));
    }
}