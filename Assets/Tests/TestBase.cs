using System.Threading.Tasks;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace DebugToolkit.Tests
{
    public abstract class TestBase
    {
        private readonly InputTestFixture _input = new();

        public virtual void OneTimeSetUp()
        {
            var gameView = GetGameView();
            gameView.minSize = new Vector2(1920, 1080);
            gameView.maxSize = new Vector2(1920, 1080);
            gameView.Focus();
            gameView.position = new Rect(0, 0, 1920, 1080);
        }

        public virtual async Task SetUp()
        {
            var document = Object.FindAnyObjectByType<UIDocument>();
            if (document != null)
            {
                Object.DestroyImmediate(document.gameObject);
            }

            _input.Setup();
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            await SceneManager.LoadSceneAsync("DefaultTests", LoadSceneMode.Additive);
        }

        public virtual async Task TearDown()
        {
            _input.TearDown();
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.ResetAndDisableNonBackgroundDevices;
            var testScene = SceneManager.GetSceneByName("DefaultTests");
            if (testScene.isLoaded)
            {
                await SceneManager.UnloadSceneAsync(testScene);
            }
        }

        [Test]
        public void InputNullTest() => Assert.That(_input, Is.Not.Null);

        private static EditorWindow GetGameView()
            => EditorWindow.GetWindow(System.Type.GetType("UnityEditor.GameView,UnityEditor"));

        protected async Awaitable ClickAtPositionAsync(Mouse mouse, Vector2 position)
        {
            _input.Set(mouse.position, position);
            _input.Click(mouse.leftButton);
            // Wait for ensure InputSystem events are processed
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();
        }

        protected async Awaitable ScrollAtPositionAsync(Mouse mouse, Vector2 position, Vector2 scrollDelta)
        {
            _input.Set(mouse.position, position);
            _input.Set(mouse.scroll, scrollDelta);
            await Awaitable.NextFrameAsync();
        }
    }
}