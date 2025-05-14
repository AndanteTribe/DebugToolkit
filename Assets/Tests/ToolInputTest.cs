using System.Threading.Tasks;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace DebugToolkit.Tests
{
    public class ToolInputTest
    {
        private readonly InputTestFixture _input = new();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var gameView = GetGameView();
            gameView.minSize = new Vector2(1920, 1080);
            gameView.position = new Rect(0, 0, 1920, 1080);
        }

        [SetUp]
        public void SetUp()
        {
            var document = Object.FindAnyObjectByType<UIDocument>();
            if (document != null)
            {
                Object.DestroyImmediate(document.gameObject);
            }
            DebugViewerBase.MasterWindow = null;

            _input.Setup();
            // Make sure it works in the background, just to be sure.
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
        }

        [TearDown]
        public void TearDown()
        {
            _input.TearDown();
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.ResetAndDisableNonBackgroundDevices;
            var mainScene = SceneManager.GetSceneByName("Main");
            if (mainScene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(mainScene);
            }
        }

        [Test]
        public void NullTest()
        {
            Assert.That(_input, Is.Not.Null);
        }

        [Test]
        [TestCase(0, 100, 920)]
        [TestCase(1, 100, 880)]
        public async Task OnClickTest(int btnIndex, float screenPosX, float screenPosY)
        {
            await SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);
            // Wait for UI drawing reflection
            await Awaitable.NextFrameAsync();

            var master = DebugViewerBase.MasterWindow;
            var contents = master.Q<VisualElement>("unity-content-container", "unity-scroll-view__content-container");
            var btns = contents.Query<Button>().ToList();
            var clicked = false;
            btns[btnIndex].clicked += () => clicked = true;

            // Wait for UI drawing reflection
            await Awaitable.NextFrameAsync();

            var mouse = InputSystem.AddDevice<Mouse>();
            _input.Set(mouse.position, new Vector2(screenPosX, screenPosY));
            _input.Click(mouse.leftButton);

            // Wait for UI drawing reflection
            await Awaitable.NextFrameAsync();
            await Awaitable.WaitForSecondsAsync(3);

            Assert.That(clicked, Is.True);
        }

        private static EditorWindow GetGameView()
            => EditorWindow.GetWindow(System.Type.GetType("UnityEditor.GameView,UnityEditor"));
    }
}