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
            DebugViewerBase.MasterWindow = null;
        }

        [Test]
        public void InputNullTest()
        {
            Assert.That(Input, Is.Not.Null);
        }

        private static EditorWindow GetGameView()
            => EditorWindow.GetWindow(System.Type.GetType("UnityEditor.GameView,UnityEditor"));
    }
}