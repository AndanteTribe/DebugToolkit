using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DebugToolkit
{
    public class DebugSceneActivator
    {
        private ParticleSystem _particle;
        private GameObject _cube;
        private GameObject _cubeToggleG1;
        private GameObject _cubeToggleG2;
        private GameObject _cubeToggleG3;
        private Slider _slider;
        private Text _textField;
        private Slider _vslider;
        private ParticleSystem _emitParticle;
        private RandomSpawner _randomSpawner;
        private Text _dropDownText;
        private Text _enumFieldText;
        private GameObject _hideObject;
        private Material _material;

        public void SetParticle(ParticleSystem par) => _particle = par;

        public void PlayParticle() => _particle.Play();

        public void SetCube(GameObject obj1) => _cube = obj1;

        public void Boolean1(bool b) => _cube.SetActive(b);

        public void SetCapToggle(GameObject obj,int i)
        {
            switch (i)
            {
                case 0:
                    _cubeToggleG1 = obj;
                    break;
                case 1:
                    _cubeToggleG2 = obj;
                    break;
                case 2:
                    _cubeToggleG3 = obj;
                    break;
            }
        }

        public void SetCapToggle2(GameObject obj, GameObject obj2, GameObject obj3)
        {
            _cubeToggleG1 = obj;
            _cubeToggleG2 = obj2;
            _cubeToggleG3 = obj3;
        }
        public void ShowToggle(int i)
        {
            switch (i)
            {
                case 0:
                    _cubeToggleG1.SetActive(true);
                    _cubeToggleG2.SetActive(false);
                    _cubeToggleG3.SetActive(false);
                    break;
                case 1:
                    _cubeToggleG1.SetActive(false);
                    _cubeToggleG2.SetActive(true);
                    _cubeToggleG3.SetActive(false);
                    break;
                case 2:
                    _cubeToggleG1.SetActive(false);
                    _cubeToggleG2.SetActive(false);
                    _cubeToggleG3.SetActive(true);
                    break;
            }
        }

        public void SetSlider(Slider s) => _slider = s;

        public void SetVerticalSlider(Slider s) => _vslider = s;

        public void SetVerticalSliderValue(float s) => _vslider.value = s;

        public void SetSliderValue(float f) => _slider.value = f;

        public void SetText(Text t) => _textField = t;

        public void SetTextValue(string text) => _textField.text = text;

        public void Integer1(int i) => Debug.Log(i);

        public void SetParticleEmitter(ParticleSystem p) => _emitParticle = p;

        public void ChangeParticleEmitter(int count)
        {
            var emitter=_emitParticle.emission;
            emitter.rateOverTime = count;
        }

        public void SetRandomSpawner(RandomSpawner r) => _randomSpawner = r;

        public void SetSpawnerRange(float min, float max)
        {
            _randomSpawner._minRange = min;
            _randomSpawner._maxRange = max;
        }

        public void SetDropDown(Text txt) => _dropDownText = txt;

        public void SetDropdownText(string st) => _dropDownText.text = st;

        public void SetEnum(Text txt) => _enumFieldText = txt;

        public void SetEnumText(string st) => _enumFieldText.text = st;

        public void SetHideObject(GameObject obj) => _hideObject = obj;

        public void SetHide(bool b) => _hideObject.SetActive(b);

        public void SetMaterial(Material m) => _material = m;

        public void SetMaterialColor(int v)
        {
            _material.color = v switch
            {
                0 => Color.red,
                1 => Color.green,
                2 => Color.blue,
                _ => _material.color
            };
        }

    }
}
