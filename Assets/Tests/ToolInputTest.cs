using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace DebugToolkit.Tests
{
    public class ToolInputTest
    {
        private readonly InputTestFixture _input = new();

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
        [TestCase(0, 100, 950)]
        [TestCase(1, 100, 895)]
        public async Task OnClickTest(int btnIndex, float fullHdX, float fullHdY)
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
            _input.Set(mouse.position, CastMousePosition(fullHdX, fullHdY));
            _input.Click(mouse.leftButton);

            // Wait for UI drawing reflection
            await Awaitable.NextFrameAsync();

            Assert.That(clicked, Is.True);
        }

        private static Vector2 CastMousePosition(float fullHdX, float fullHdY)
        {
            // 1920:1080 = Screen.width:Screen.height
            return new Vector2(fullHdX * Screen.width / 1920, fullHdY * Screen.height / 1080);
        }
    }
}