using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

namespace DebugToolkit.Tests
{
    public abstract class TestBase
    {
        protected readonly InputTestFixture Input = new();

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

            Input.Setup();
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            await SceneManager.LoadSceneAsync("DefaultTests", LoadSceneMode.Additive);
        }

        public virtual async Task TearDown()
        {
            Input.TearDown();
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.ResetAndDisableNonBackgroundDevices;
            var testScene = SceneManager.GetSceneByName("DefaultTests");
            if (testScene.isLoaded)
            {
                await SceneManager.UnloadSceneAsync(testScene);
            }
            // No need to clean up static fields since they are now instance-based
        }

        [Test]
        public void InputNullTest()
        {
            Assert.That(Input, Is.Not.Null);
        }

        private static EditorWindow GetGameView()
            => EditorWindow.GetWindow(System.Type.GetType("UnityEditor.GameView,UnityEditor"));

        protected async Awaitable ClickAtPositionAsync(Mouse mouse, Vector2 position)
        {
            Input.Set(mouse.position, position);
            Input.Click(mouse.leftButton);
            // Wait for two frames to ensure InputSystem events are processed
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();
        }

        protected async Awaitable ScrollAtPositionAsync(Mouse mouse, Vector2 position, Vector2 scrollDelta)
        {
            Input.Set(mouse.position, position);
            Input.Set(mouse.scroll, scrollDelta);
            await Awaitable.NextFrameAsync();
        }
    }
}
