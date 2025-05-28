using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

#if UNITY_2023_2_OR_NEWER
namespace DebugToolkit.Tests
{
    public class TabTest
    {
        private DebugViewTabTest _debugViewTabTest;
        private readonly InputTestFixture _input = new();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var gameView = GetGameView();
            gameView.minSize = new Vector2(1920, 1080);
            gameView.maxSize = new Vector2(1920, 1080);
            gameView.Focus();
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
            _debugViewTabTest = new DebugViewTabTest();
            _debugViewTabTest.Start();
            await Awaitable.NextFrameAsync();
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
            _debugViewTabTest = null;
            DebugViewerBase.MasterWindow = null;
            DebugViewerBase.DebugWindowList.Clear();
        }

        [Test]
        public void InputNullTest()
        {
            Assert.That(_input, Is.Not.Null);
        }

        // タブにスクロールビューが生えてるかテスト
        [Test]
        public void ScrollView_IsCorrectlyAddedAtTab()
        {
            var tab = _debugViewTabTest.Root.Q<Tab>();
            Assert.That(tab.Q<ScrollView>(),Is.Not.Null, "ScrollView should be added to Tab.");
        }

        // タブビューがないときにAddTabしたらTabViewが生えるかテスト
        [Test]
        public void TabView_IsCorrectlyAdded_WhenAddTab()
        {
            var anotherWindow = _debugViewTabTest.Root.AddWindow("AnotherWindow");
            var (tabRoot, tab) = anotherWindow.AddTab();
            Assert.That(anotherWindow.Q<TabView>(),Is.Not.Null, "TabView should be added to Window when AddTab.");
        }

        // タブの名前のラベルが正しく適応されているかテスト
        [Test]
        public void TabLabel_IsCorrectlySet()
        {
            var anotherWindow = _debugViewTabTest.Root.AddWindow("AnotherWindow");
            var (tabRoot, tab) = anotherWindow.AddTab("NewTab");
            Assert.That(anotherWindow.Q<Tab>(),Is.Not.Null);
            Assert.That(anotherWindow.Q<Tab>().label, Is.EqualTo("NewTab"), "Tab label should be set correctly.");
        }

        private static EditorWindow GetGameView()
            => EditorWindow.GetWindow(System.Type.GetType("UnityEditor.GameView,UnityEditor"));
    }
}
#endif
