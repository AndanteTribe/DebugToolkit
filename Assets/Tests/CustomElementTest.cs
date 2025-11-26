using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace DebugToolkit.Tests
{
    public class CustomElementTest : TestBase
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
        }

        [TearDown]
        public override async Task TearDown()
        {
            await base.TearDown();

            // Destroy the instance. In some cases, it might be better not to do this.
            _debugViewCustomElementTest= null;
        }

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
        public async Task ConsoleView_SearchAndCategorizeMessages_WorksCorrectly(
            int positionX0, int positionX1, int positionX2, LogType type)
        {
            ConsoleView.Initialize();

            var window = _debugViewCustomElementTest.Root.AddWindow("TestWindow");
            window.parent.style.display = DisplayStyle.Flex;
            var consoleView = window.AddConsoleView();
            Assert.That(consoleView, Is.Not.Null, "ConsoleView has not been created.");

            Debug.Log("Test log message");
            Debug.LogWarning("Test warning message");
            LogAssert.Expect("Test error message");
            Debug.LogError("Test error message");

            await Awaitable.NextFrameAsync();

            var listView = consoleView.Q<ListView>();
            Assert.That(listView, Is.Not.Null, "The ListView does not exist.");
            var items = listView.itemsSource;
            Assert.That(items, Is.Not.Null, "The list items are not created.");
            Assert.That(items.Count, Is.GreaterThanOrEqualTo(3), "There are fewer than 3 items in the ListView.");

            var mouse = InputSystem.AddDevice<Mouse>();
            await ClickAtPositionAsync(mouse, new Vector2(positionX0, 824));

            var filteredItems = listView.itemsSource;
            Assert.That(filteredItems, Is.Not.Null, "1: The type of the filter result is incorrect.");
            Assert.That(filteredItems.Count, Is.EqualTo(2), "1: The search results are not filtered correctly.");

            await ClickAtPositionAsync(mouse, new Vector2(positionX1, 824));

            filteredItems = listView.itemsSource;
            Assert.That(filteredItems, Is.Not.Null, "2: The type of the filter result is incorrect.");
            Assert.That(filteredItems.Count, Is.EqualTo(1), "2: The search results are not filtered correctly.");
            var logEntry = filteredItems[0] as (string, string, LogType, DateTime)?;
            Assert.That(logEntry?.Item3, Is.EqualTo(type), "2: The log type of the filter result is not a warning.");

            await ClickAtPositionAsync(mouse, new Vector2(positionX2, 824));

            filteredItems = listView.itemsSource;
            Assert.That(filteredItems, Is.Not.Null, "3: The type of the filter result is incorrect.");
            Assert.That(filteredItems.Count, Is.EqualTo(0), "3: The search results are not filtered correctly.");
        }
    }
}
