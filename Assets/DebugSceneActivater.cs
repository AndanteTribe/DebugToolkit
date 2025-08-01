using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugSceneActivater : MonoBehaviour,IDebugInterface
{
    ParticleSystem _particle;
    GameObject _cube;
    GameObject _cubeToggleG1;
    GameObject _cubeToggleG2;
    GameObject _cubeToggleG3;
    Slider _slider;
    Text _textField;
    private Slider _vslider;
    private ParticleSystem _emitParticle;
    RandomSpawner _randomSpawner;
    public Text dropDownText;
    public Text enumFieldText;
    public GameObject hideObject;
    public Material material;


    /*
     * 標準フレームワークと言語のバージョンは違うぞ
     * 例えばint型。言語で定義されているわけではなく、フレームワークで定義。
     * public,privateは言語。classとか{get;set;}もc#。
     * void での return はフレームワークで入所、配布している。
     * 密接に関わっているもの。疎結合であるものの２つ。
     * javaはバージョンそろえないとダメ。密接。
     * C#にはコンパイラーがいる。
     * List
     */

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

    public void SetDropdownText(string st)
    {
        dropDownText.text = st;
    }

    public void SetEnumText(string st)
    {
        enumFieldText.text = st;
    }

    public void SetHide(bool b)
    {
        hideObject.SetActive(b);
    }

    public void SetMaterial(int v)
    {
        if (v == 0) material.color = Color.red;
        if (v == 1) material.color = Color.green;
        if (v == 2) material.color = Color.blue;
    }

}
