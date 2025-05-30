using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace DebugToolkit.Tests
{
    public class UssTest : TestBase
    {
        private DebugViewTestBase _debugViewUssTest;

        [OneTimeSetUp]
        public override void OneTimeSetUp()
        {
            var directoryPath = Path.Combine(Application.dataPath, "..", "artifacts-screenshot");
            Directory.CreateDirectory(directoryPath);

            base.OneTimeSetUp();
        }

        [SetUp]
        public override async Task SetUp()
        {
            await base.SetUp();
            _debugViewUssTest = new DebugViewTestBase();
            _debugViewUssTest.Start();
        }

        [TearDown]
        public override async Task TearDown()
        {
            await base.TearDown();

            //  インスタンスの破棄、場合によってはやめた方がいいかも？
            _debugViewUssTest = null;
        }

        [Test]
        public async Task UssWindow_AllElementsTest()
        {
            var window = _debugViewUssTest.Root.AddWindow("UssWindowTest");
            var scrollview = new ScrollView();
            window.Add(scrollview);
            AddAllUIElements(scrollview);
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            var mouse = InputSystem.AddDevice<Mouse>();
            Input.Set(mouse.position, new Vector2(110, 930));
            Input.Click(mouse.leftButton);

            await CaptureScreenAsync(nameof(UssWindow_AllElementsTest) + "_scroll-before");

            Input.Set(mouse.position, new Vector2(525, 430));
            Input.Set(mouse.scroll, new Vector2(0, -100));

            await CaptureScreenAsync(nameof(UssWindow_AllElementsTest) + "_scroll-after");

            Assert.That(window.Q(className: DebugConst.WindowContentClassName).childCount, Is.GreaterThan(0));
        }

        [Test]
        public async Task UssTab_AllElementsTest()
        {
            var window = _debugViewUssTest.Root.AddWindow("UssTabTest");
            var (tabRoot, tab1) = window.AddTab("Test");
            AddAllUIElements(tab1);
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();

            var mouse = InputSystem.AddDevice<Mouse>();
            Input.Set(mouse.position, new Vector2(110, 930));
            Input.Click(mouse.leftButton);

            await CaptureScreenAsync(nameof(UssTab_AllElementsTest) + "_scroll-before");

            Input.Set(mouse.position, new Vector2(525, 430));
            Input.Set(mouse.scroll, new Vector2(0, -100));

            await CaptureScreenAsync(nameof(UssTab_AllElementsTest) + "_scroll-after");

            Assert.That(tab1.childCount, Is.GreaterThan(0));
        }

        private static async Awaitable<string> CaptureScreenAsync(string fineName)
        {
            var directoryPath = Path.Combine(Application.dataPath, "..", "artifacts-screenshot");

            await Awaitable.EndOfFrameAsync();
            await Awaitable.NextFrameAsync();
            var result = Path.Combine(directoryPath, fineName + ".png");
            ScreenCapture.CaptureScreenshot(result);
            Debug.Log($"Screenshot captured: {result}");
            return result;
        }

        private enum TestEnum
        {
            ValueA,
            ValueB,
            ValueC
        }

        private static void AddAllUIElements(VisualElement container)
        {
            // Label
            var label = new Label { text = "Test Label" };
            container.Add(label);

            // Button
            var button = new Button { text = "Test Button" };
            button.clicked += () => Debug.Log("Button Clicked");
            container.Add(button);

            // Toggle
            var toggle = new Toggle { label = "Test Toggle" };
            toggle.RegisterValueChangedCallback(evt => Debug.Log($"Toggle Value Changed: {evt.newValue}"));
            container.Add(toggle);

            // Toggle Button Group
            var toggleGroup = new ToggleButtonGroup() { label = "Test Toggle Button Group" };
            var toggle1 = new Toggle { label = "Test Toggle 1" };
            toggle1.RegisterValueChangedCallback(evt => Debug.Log($"Toggle 1 Value Changed: {evt.newValue}"));
            toggleGroup.Add(toggle1);
            var toggle2 = new Toggle { label = "Test Toggle 2" };
            toggle2.RegisterValueChangedCallback(evt => Debug.Log($"Toggle 2 Value Changed: {evt.newValue}"));
            toggleGroup.Add(toggle2);
            var toggle3 = new Toggle { label = "Test Toggle 3" };
            toggle3.RegisterValueChangedCallback(evt => Debug.Log($"Toggle 3 Value Changed: {evt.newValue}"));
            toggleGroup.Add(toggle3);
            container.Add(toggleGroup);

            // TextField
            var textField = new TextField { label = "Test TextField" };
            textField.RegisterValueChangedCallback(evt => Debug.Log($"TextField Value Changed: {evt.newValue}"));
            container.Add(textField);

            // Foldout
            var foldout = new Foldout { text = "Test Foldout" };
            var foldoutContent = new Label("Foldout Content");
            foldout.Add(foldoutContent);
            container.Add(foldout);

            // Slider
            var slider = new Slider(0, 100) { label = "Test Slider", showInputField = true };
            slider.RegisterValueChangedCallback(evt => Debug.Log($"Slider Value Changed: {evt.newValue}"));
            container.Add(slider);

            // SliderInt
            var sliderInt = new SliderInt(0, 100) { label = "Test SliderInt", showInputField = true };
            sliderInt.RegisterValueChangedCallback(evt => Debug.Log($"SliderInt Value Changed: {evt.newValue}"));
            container.Add(sliderInt);

            // MinMaxSlider
            var minMaxSlider = new MinMaxSlider("Test MinMaxSlider", 0, 100, 25, 75);
            minMaxSlider.RegisterValueChangedCallback(evt => Debug.Log($"MinMaxSlider Value Changed: {evt.newValue}"));
            container.Add(minMaxSlider);

            // ProgressBar
            var progressBar = new ProgressBar { title = "Test ProgressBar", lowValue = 0, highValue = 100, value = 50 };
            container.Add(progressBar);

            // Dropdown
            var choices = new List<string> { "Choice 1", "Choice 2", "Choice 3" };
            var dropdown = new DropdownField("Test Dropdown", choices, 0);
            dropdown.RegisterValueChangedCallback(evt => Debug.Log($"Dropdown Value Changed: {evt.newValue}"));
            container.Add(dropdown);

            // EnumField
            var enumField = new EnumField("Test EnumField", TestEnum.ValueA);
            enumField.RegisterValueChangedCallback(evt => Debug.Log($"EnumField Value Changed: {evt.newValue}"));
            container.Add(enumField);

            // RadioButton
            var radioButton = new RadioButton { label = "Test RadioButton" };
            radioButton.RegisterValueChangedCallback(evt => Debug.Log($"RadioButton Value Changed: {evt.newValue}"));
            container.Add(radioButton);

            // RadioButtonGroup
            var radioButtonGroup = new RadioButtonGroup("Test RadioButtonGroup");
            var rb1 = new RadioButton("RB Option 1");
            var rb2 = new RadioButton("RB Option 2");
            var rb3 = new RadioButton("RB Option 3");
            radioButtonGroup.Add(rb1);
            radioButtonGroup.Add(rb2);
            radioButtonGroup.Add(rb3);
            container.Add(radioButtonGroup);
        }
    }
}



