using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugSceneActivator : MonoBehaviour
{
    private ParticleSystem _particle;
    GameObject _cube;
    GameObject _cubeToggleG1;
    GameObject _cubeToggleG2;
    GameObject _cubeToggleG3;
    Slider _slider;
    Text _textField;
    private Slider _vslider;
    private ParticleSystem _emitParticle;
    RandomSpawner _randomSpawner;
    private Text _dropDownText;
    private Text _enumFieldText;
    private GameObject _hideObject;
    private Material _material;

    public void SetParticle(ParticleSystem par)
    {
        _particle = par;
    }

    public void PlayParticle()
    {
        _particle.Play();
    }

    public void SetCube(GameObject obj1)
    {
        _cube = obj1;
    }
    public void Boolean1(bool b)
    {
        _cube.SetActive(b);
    }
    public void SetCapToggle(GameObject obj,int i)
    {
        if (i == 0) _cubeToggleG1 = obj;
        if (i == 1) _cubeToggleG2 = obj;
        if (i == 2) _cubeToggleG3 = obj;
    }
    public void ShowToggle(int i)
    {
        if (i == 0) _cubeToggleG1.SetActive(true);
        else _cubeToggleG1.SetActive(false);
        if (i == 1) _cubeToggleG2.SetActive(true);
        else _cubeToggleG2.SetActive(false);
        if (i == 2) _cubeToggleG3.SetActive(true);
        else _cubeToggleG3.SetActive(false);
    }

    public void SetSlider(Slider s)
    {
        _slider = s;
    }

    public void SetVerticalSlider(Slider s)
    {
        _vslider = s;
    }

    public void SetVerticalSliderValue(float s)
    {
        _vslider.value = s;
    }

    public void SetSliderValue(float f)
    {
        _slider.value = f;
    }

    public void SetText(Text t)
    {
        _textField = t;
    }

    public void SetTextValue(string text)
    {
        _textField.text = text;
    }

    public void Integer1(int i)
    {
        Debug.Log(i);
    }

    public void SetParticleEmitter(ParticleSystem p)
    {
        _emitParticle = p;
    }
    public void ChangeParticleEmitter(int count)
    {
        var emitter=_emitParticle.emission;
        emitter.rateOverTime = count;
    }

    public void SetRandomSpawner(RandomSpawner r)
    {
        _randomSpawner = r;
    }

    public void SetSpawnerRange(float min, float max)
    {
        _randomSpawner.minRange = min;
        _randomSpawner.maxRange = max;
    }

    public void SetDropDown(Text txt)
    {
        _dropDownText = txt;
    }

    public void SetDropdownText(string st)
    {
        _dropDownText.text = st;
    }
    public void SetEnum(Text txt)
    {
        _enumFieldText = txt;
    }

    public void SetEnumText(string st)
    {
        _enumFieldText.text = st;
    }

    public void SetHideObject(GameObject obj)
    {
        _hideObject = obj;
    }

    public void SetHide(bool b)
    {
        _hideObject.SetActive(b);
    }

    public void SetMaterial(Material m)
    {
        _material = m;
    }

    public void SetMaterialColor(int v)
    {
        if (v == 0) _material.color = Color.red;
        if (v == 1) _material.color = Color.green;
        if (v == 2) _material.color = Color.blue;
    }

}
