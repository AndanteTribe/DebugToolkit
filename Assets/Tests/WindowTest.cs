using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using Task = System.Threading.Tasks.Task;

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

        //  いずれマスターウィンドウをstaticじゃなくするときは変更する
        [Test]
        public void MasterWindow_IsCorrectlyGenerated()
        {
            Assert.That(DebugViewerBase.MasterWindow, Is.Not.Null, "MasterWindow should be generated.");
            Assert.That(DebugViewerBase.MasterWindow.parent.Q<Label>(className: DebugConst.DebugToolkitWindowLabelClassName).text, Is.EqualTo("Debug Toolkit"), "MasterWindow title is incorrect.");
        }

        private static EditorWindow GetGameView()
            => EditorWindow.GetWindow(System.Type.GetType("UnityEditor.GameView,UnityEditor"));
    }
}