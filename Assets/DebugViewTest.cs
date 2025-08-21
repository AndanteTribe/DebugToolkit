using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        float _minXValue = 1f;
        float _maXValue = 100f;
        DebugSceneActivator _debugSceneActivator=new DebugSceneActivator();

        public DebugViewTest(ParticleSystem par,GameObject obj1,
            GameObject objToG1,GameObject objToG2,GameObject objToG3,
            Slider sl,Text txt,Slider vSlider,ParticleSystem emitter,RandomSpawner randomSpawner,
            Text dropText,Text enumText,GameObject hider,Material mat)
        {
            _debugSceneActivator.SetParticle(par);
            _debugSceneActivator.SetCube(obj1);
            _debugSceneActivator.SetCapToggle(objToG1,0);
            _debugSceneActivator.SetCapToggle(objToG2,1);
            _debugSceneActivator.SetCapToggle(objToG3,2);
            _debugSceneActivator.SetCapToggle2(objToG1,objToG2,objToG3);
            _debugSceneActivator.SetSlider(sl);
            _debugSceneActivator.SetText(txt);
            _debugSceneActivator.SetVerticalSlider(vSlider);
            _debugSceneActivator.SetParticleEmitter(emitter);
            _debugSceneActivator.SetRandomSpawner(randomSpawner);
            _debugSceneActivator.SetMaterial(mat);
            _debugSceneActivator.SetHideObject(hider);
            _debugSceneActivator.SetDropDown(dropText);
            _debugSceneActivator.SetEnum(enumText);
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

            var particleEmitButton = new Button() { text = "Particle Emitter" };
            particleEmitButton.clicked += () =>
            {
                _debugSceneActivator.PlayParticle();
            };
            tab1.Add(particleEmitButton);

            var toggle = new Toggle() { text = $"Toggle:false" };
            toggle.RegisterValueChangedCallbackWithTracking((evt) =>
            {
                _debugSceneActivator.Boolean1(evt.newValue);
            });
            tab1.Add(toggle);

            var toggleGroup = new ToggleButtonGroup();
            toggleGroup.label="ToggleGroup";
            var toggleG1=new Toggle(){text = "Toggle 1"};
            toggleG1.RegisterValueChangedCallback(evt =>
            {
                _debugSceneActivator.ShowToggle(0);
            });
            var toggleG2=new Toggle(){text = "Toggle 2"};
            toggleG2.RegisterValueChangedCallback(evt =>
            {
                _debugSceneActivator.ShowToggle(1);
            });
            var toggleG3=new Toggle(){text = "Toggle 3"};
            toggleG3.RegisterValueChangedCallback(evt =>
            {
                _debugSceneActivator.ShowToggle(2);
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
                _debugSceneActivator.SetSliderValue(value);
                ballBlocker.style.left = value;
                ballBlockerLabel.text = $"Ball Blocker Position: {ballBlocker.style.left}";
            };
            tab1.Add(ballBlocker);
            tab1.Add(horizontalScroller);
            tab1.Add(minLabel);
            tab1.Add(maxLabel);
            tab1.Add(ballBlockerLabel);

            var textField = new TextField() { label = "TextField:" };
            textField.RegisterValueChangedCallbackWithTracking((v) =>
            {
                _debugSceneActivator.SetTextValue(v.newValue);
            });
            tab1.Add(textField);


            var foldOut=new Foldout();
            foldOut.text = "FoldOut 1";
            var textInFold1=new Label() { text="TextInFold1" };
            var particleEmitButtonInFold = new Button() { text = "Particle Emitter" };
            particleEmitButtonInFold.clicked += () =>
            {
                _debugSceneActivator.PlayParticle();
            };
            foldOut.contentContainer.Add(particleEmitButtonInFold);
            foldOut.contentContainer.Add(textInFold1);
            var foldOut2=new Foldout();
            foldOut2.text = "FoldOut2";
            var textInFold2=new Label() { text="TextInFold2" };
            var particleEmitButtonInFold2 = new Button() { text = "Particle Emitter" };
            particleEmitButtonInFold2.clicked += () =>
            {
                _debugSceneActivator.PlayParticle();
            };
            foldOut2.contentContainer.Add(particleEmitButtonInFold2);
            foldOut2.contentContainer.Add(textInFold2);
            foldOut2.value = false;
            foldOut.contentContainer.Add(foldOut2);
            foldOut.value = false;
            tab1.Add(foldOut);

            var progressBar = new ProgressBar();
            progressBar.title = "Progress";
            progressBar.lowValue = 0f;
            progressBar.value = 0f;
            progressBar.highValue = 100f;

            var slider = new UnityEngine.UIElements.Slider();
            slider.highValue = 100f;
            slider.lowValue = 0;
            slider.label = "Progress Slider";
            slider.RegisterValueChangedCallbackWithTracking(evt =>
            {
                progressBar.value = evt.newValue;
                _debugSceneActivator.SetVerticalSliderValue(evt.newValue);
            });
            tab1.Add(slider);
            var sliderInt = new SliderInt();
            sliderInt.highValue = 10;
            sliderInt.lowValue = 1;
            sliderInt.label = "Particle Count";
            sliderInt.RegisterValueChangedCallbackWithTracking(evt =>
            {
                _debugSceneActivator.ChangeParticleEmitter(evt.newValue);
            });
            tab1.Add(sliderInt);

            var minMaxSlider = new MinMaxSlider();
            minMaxSlider.highLimit = 10f;
            minMaxSlider.lowLimit = -10;
            _debugSceneActivator.SetSpawnerRange(minMaxSlider.value.x, minMaxSlider.value.y);
            minMaxSlider.RegisterValueChangedCallback(evt =>
            {
                _debugSceneActivator.SetSpawnerRange(minMaxSlider.value.x, minMaxSlider.value.y);
            });
            tab1.Add(minMaxSlider);


            tab1.Add(progressBar);

            var dropdown = new DropdownField();
            dropdown.label = "Dropdown:";
            dropdown.choices = new List<string>() {"Choice 1","Choice 2","Choice 3" };
            dropdown.RegisterValueChangedCallbackWithTracking(evt =>
            {
                _debugSceneActivator.SetDropdownText(evt.newValue);
            });
            tab1.Add(dropdown);

            var enumField=new EnumField("EnumField :",EnumTypes.Enum1);
            enumField.RegisterValueChangedCallbackWithTracking(evt =>
            {
                _debugSceneActivator.SetEnumText(evt.newValue.ToString());
            });
            tab1.Add(enumField);

            var radioButton = new RadioButton();
            radioButton.label = "RadioButton:";
            radioButton.RegisterValueChangedCallback(evt =>
            {
                _debugSceneActivator.SetHide(evt.newValue);
            });
            tab1.Add(radioButton);

            var radioGroup = new RadioButtonGroup();
            radioGroup.choices = new List<string>() {"red","green","blue"};
            radioGroup.RegisterValueChangedCallback(t =>
            {
                _debugSceneActivator.SetMaterialColor(radioGroup.value);
            });
            tab1.Add(radioGroup);

            //tab2の追加、コンソールのみになっている
            tab2.AddProfileInfoLabel();

            var tab3 = tabRoot.AddTab(label:"Tab3");
            var tab3Label = new Label() { text = "This is Tab3." };
            tab3.Add(tab3Label);

            var window2 = root.AddWindow("TestWindow2");
            window2.AddConsoleView();

            var window3 = root.AddWindow("TestWindow3");
            var window3Label = new Label() { text = "This is Window3." };
            window3.Add(window3Label);

            // Add change log window to demonstrate change visualization
            var changeLogWindow = root.AddWindow("Change Log");
            changeLogWindow.AddChangeLogView();

            return root;
        }
    }
}
