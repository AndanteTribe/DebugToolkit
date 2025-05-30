using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.TestTools;

namespace DebugToolkit.Tests
{
    public class CustomElementTest: TestBase
    {
        private DebugViewTestBase _debugViewCustomElementTest;

        [OneTimeSetUp]
        public override void OneTimeSetUp() => base.OneTimeSetUp();

        [SetUp]
        public override async Task SetUp()
        {
            await base.SetUp();
            _debugViewCustomElementTest = new DebugViewTestBase();
            _debugViewCustomElementTest.Start();
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();
        }

        [TearDown]
        public override async Task TearDown()
        {
            await base.TearDown();

            //  インスタンスの破棄、場合によってはやめた方がいいかも？
            _debugViewCustomElementTest= null;
            DebugViewerBase.MasterWindow = null;
            DebugViewerBase.DebugWindowList.Clear();
        }

        private static EditorWindow GetGameView()
            => EditorWindow.GetWindow(System.Type.GetType("UnityEditor.GameView,UnityEditor"));

        [Test]
        public async Task AddProfileInfoLabel_DisplaysCorrectProfileInfo()
        {
            var root = _debugViewCustomElementTest.Root;
            var window = root.AddWindow("TestWindow");
            window.parent.style.display = DisplayStyle.Flex;
            window.AddProfileInfoLabel();
            var label = window.Q<Label>();

            Assert.That(label, Is.Not.Null, "label should be added");
            await Awaitable.EndOfFrameAsync();

            var expectedMemory = ProfileUtils.GetTotalMemoryGB();
            var frameTiming = ProfileUtils.GetLatestFrameTiming();
            var expectedMemoryString = expectedMemory.ToString("F2");
            var expectedCpuFpsString =  (1000 / frameTiming.cpuFrameTime).ToString("F0");
            var expectedCpuFrameTimeString = frameTiming.cpuFrameTime.ToString("F1");
            var expectedGpuFpsString = (1000 / frameTiming.gpuFrameTime).ToString("F0");
            var expectedGpuFrameTimeString =  frameTiming.gpuFrameTime.ToString("F1");

            Assert.That(label.text, Does.Contain(expectedMemoryString), "memory value should be contained in label.");
            Assert.That(label.text, Does.Contain(expectedCpuFpsString), "cpu fps value should be contained in label.");
            Assert.That(label.text, Does.Contain(expectedCpuFrameTimeString), "cpu frame time value should be contained in label.");
            Assert.That(label.text, Does.Contain(expectedGpuFpsString), "gpu fps value should be contained in label.");
            Assert.That(label.text, Does.Contain(expectedGpuFrameTimeString), "gpu frame time value should be contained in label.");
        }

        [Test]
        [TestCase (220, 332, 441, LogType.Error)]
        [TestCase(220, 441, 332, LogType.Warning)]
        [TestCase(332, 441, 220, LogType.Log)]
        [TestCase(332, 220, 441, LogType.Error)]
        [TestCase(441, 220, 332, LogType.Warning)]
        [TestCase(441, 332, 220, LogType.Log)]
        public async Task ConsoleVIew_SearchAndCategorizeMessages_WorksCorrectly(
            int positionX0, int positionX1, int positionX2, LogType type)
        {
            var window = _debugViewCustomElementTest.Root.AddWindow("TestWindow");
            window.parent.style.display = DisplayStyle.Flex;
            var consoleView = window.AddConsoleView();
            Assert.That(consoleView, Is.Not.Null, "ConsoleView has not been created.");

            Debug.Log("Test log message");
            Debug.LogWarning("Test warning message");
            LogAssert.Expect("Test error message");
            Debug.LogError("Test error message");

            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            var listView = consoleView.Q<ListView>();
            Assert.That(listView, Is.Not.Null, "The ListView does not exist.");
            var items = listView.itemsSource as List<(string message, string stackTrace, LogType type)>;
            Assert.That(items, Is.Not.Null.And.Count.GreaterThanOrEqualTo(3), "There are fewer than 3 items in the ListView.");

            var mouse = InputSystem.AddDevice<Mouse>();
            Input.Set(mouse.position, new Vector2(positionX0, 824));
            Input.Click(mouse.leftButton);
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            var filteredItems = listView.itemsSource as List<(string message, string stackTrace, LogType type)>;
            Assert.That(filteredItems, Is.Not.Null, "1: The type of the filter result is incorrect.");
            Assert.That(filteredItems.Count, Is.EqualTo(2), "1: The search results are not filtered correctly.");

            Input.Set(mouse.position, new Vector2(positionX1, 824));
            Input.Click(mouse.leftButton);
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            filteredItems = listView.itemsSource as List<(string message, string stackTrace, LogType type)>;
            Assert.That(filteredItems, Is.Not.Null, "2: The type of the filter result is incorrect.");
            Assert.That(filteredItems.Count, Is.EqualTo(1), "2: The search results are not filtered correctly.");
            Assert.That(filteredItems[0].type, Is.EqualTo(type), "2: The log type of the filter result is not a warning.");

            Input.Set(mouse.position, new Vector2(positionX2, 824));
            Input.Click(mouse.leftButton);
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            filteredItems = listView.itemsSource as List<(string message, string stackTrace, LogType type)>;
            Assert.That(filteredItems, Is.Not.Null, "3: The type of the filter result is incorrect.");
            Assert.That(filteredItems.Count, Is.EqualTo(0), "3: The search results are not filtered correctly.");
        }
    }
}
