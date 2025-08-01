using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UIElements.Toggle;

namespace DebugToolkit
{
    public class DebugViewTest : DebugViewerBase
    {
        bool _objectFinding = false;
        float _minXValue = 1f;
        float _maXValue = 100f;
        DebugSceneActivater _debugSceneActivater=new DebugSceneActivater();

        public DebugViewTest(ParticleSystem par,GameObject obj1,
            GameObject objToG1,GameObject objToG2,GameObject objToG3)
        {
            _debugSceneActivater.SetParticle(par);
            _debugSceneActivater.SetCube(obj1);
            _debugSceneActivater.SetCapToggle(objToG1,0);
            _debugSceneActivater.SetCapToggle(objToG2,1);
            _debugSceneActivater.SetCapToggle(objToG3,2);
        }


        public enum EnumTypes
        {
            Enum1,
            Enum2,
            Enum3
        }

        protected override VisualElement CreateViewGUI()
        {
            //rootが最初のウィンドウ表示
            var root = base.CreateViewGUI();
            var window = root.AddWindow("TestWindow");
            var (tabRoot, tab1) = window.AddTab("Tab1");
            var tab2 = tabRoot.AddTab("Tab2");



            var testText = new Label() { text = "hoge" };
            tab1.Add(testText);

            var objectFinderButton = new Button() { text = "Object Finder" };
            objectFinderButton.clicked += () =>
            {
                _debugSceneActivater.PlayParticle();
            };
            tab1.Add(objectFinderButton);

            var toggle = new Toggle() { text = $"Toggle:false" };
            toggle.RegisterValueChangedCallback((evt) =>
            {
                _debugSceneActivater.Boolean1(evt.newValue);
            });
            tab1.Add(toggle);

            var toggleGroup = new ToggleButtonGroup();
            toggleGroup.label="ToggleGroup";
            var toggleG1=new Toggle(){text = "Toggle 1"};
            toggleG1.RegisterValueChangedCallback(evt =>
            {
                _debugSceneActivater.ShowToggle(0);
            });
            var toggleG2=new Toggle(){text = "Toggle 2"};
            toggleG2.RegisterValueChangedCallback(evt =>
            {
                _debugSceneActivater.ShowToggle(1);
            });
            var toggleG3=new Toggle(){text = "Toggle 3"};
            toggleG3.RegisterValueChangedCallback(evt =>
            {
                _debugSceneActivater.ShowToggle(2);
            });
            toggleGroup.Add(toggleG1);
            toggleGroup.Add(toggleG2);
            toggleGroup.Add(toggleG3);
            toggleGroup.isMultipleSelection = false;
            tab1.Add(toggleGroup);


            var ballBlocker = new VisualElement();
            var horizontalScroller = new Scroller(_minXValue, _maXValue, null, SliderDirection.Horizontal);
            var minLabel = new Label($"Minimum Value: {_minXValue}");
            var maxLabel = new Label($"Maximum Value: {_maXValue}");
            var ballBlockerLabel = new Label($"Ball Blocker Position: {ballBlocker.style.left}");
            ballBlocker.style.left = horizontalScroller.value;

            // The width of the Scroller is independent of its value range.
            horizontalScroller.style.width = 300;
            horizontalScroller.valueChanged += (value) =>
            {
                ballBlocker.style.left = value;
                ballBlockerLabel.text = $"Ball Blocker Position: {ballBlocker.style.left}";
            };
            tab1.Add(ballBlocker);
            tab1.Add(horizontalScroller);
            tab1.Add(minLabel);
            tab1.Add(maxLabel);
            tab1.Add(ballBlockerLabel);

            var textField = new TextField() { label = "TextField:" };
            var textLabel = new Label() { text = $"Label:" };
            var textButton=new Button() { text = "Change Label" };
            textButton.clicked += () => textLabel.text = $"Label: {textField.value}";
            tab1.Add(textField);
            tab1.Add(textLabel);
            tab1.Add(textButton);


            var foldOut=new Foldout();
            foldOut.text = "FoldOut 1";
            var textInFold1=new Label() { text="TextInFold1" };
            foldOut.contentContainer.Add(textInFold1);
            var foldOut2=new Foldout();
            foldOut2.text = "FoldOut2";
            var textInFold2=new Label() { text="TextInFold2" };
            foldOut2.contentContainer.Add(textInFold2);
            foldOut2.value = false;
            foldOut.contentContainer.Add(foldOut2);
            foldOut.value = false;
            tab1.Add(foldOut);

            var slider = new UnityEngine.UIElements.Slider();
            slider.highValue = 111.1f;
            slider.lowValue = 0;
            tab1.Add(slider);
            var sliderInt = new SliderInt();
            sliderInt.highValue = 10;
            sliderInt.lowValue = 0;
            tab1.Add(sliderInt);
            var minMaxSlider = new MinMaxSlider();
            minMaxSlider.maxValue = 100f;
            minMaxSlider.minValue = 0;
            tab1.Add(minMaxSlider);

            var progressBar = new ProgressBar();
            progressBar.title = "Progress";
            progressBar.lowValue = 0f;
            progressBar.value = 0f;
            progressBar.highValue = 100f;
            tab1.Add(progressBar);

            var dropdown = new DropdownField();
            dropdown.label = "Dropdown:";
            dropdown.choices = new List<string>() {"Choice 1","Choice 2","Choice 3" };
            tab1.Add(dropdown);

            var enumField=new EnumField("EnumField :",EnumTypes.Enum1);
            tab1.Add(enumField);

            var radioButton = new RadioButton();
            radioButton.label = "RadioButton:";
            tab1.Add(radioButton);

            var radioGroup = new RadioButtonGroup();
            radioGroup.choices = new List<string>() {"Radio 1","Radio 2","Radio 3"};
            radioGroup.RegisterValueChangedCallback(t =>
            {
                string answerValue = t.newValue.ToString();
                Debug.Log($"Pushed :{answerValue}");
            });
            tab1.Add(radioGroup);

            // var testButton = new Button(){text = "Hoge"};
            // testButton.clicked += () => Debug.Log("TestButton");
            // tab1.Add(testButton);
            // var logButton = new Button(){ text = "Log" };
            // logButton.clicked += () => Debug.Log("Log test message");
            // tab1.Add(logButton);
            //
            // var warningButton = new Button(){ text = "Warning" };
            // warningButton.clicked += () => Debug.LogWarning("Warning test message");
            // tab1.Add(warningButton);
            //
            // var errorButton = new Button(){ text = "Error" };
            // errorButton.clicked += () => Debug.LogError("Error test message");
            // tab1.Add(errorButton);
            //
            // for (int i = 0; i < 10; i++)
            // {
            //     tab1.Add(new Button(){text = "hogehoge"});
            // }

            //tab2の追加、コンソールのみになっている
            tab2.AddProfileInfoLabel();
            var window2 = root.AddWindow("TestWindow2");
            window2.AddConsoleView();
            return root;
        }
    }
}