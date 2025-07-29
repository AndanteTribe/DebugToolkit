using UnityEngine;

public class DebugSceneActivater : MonoBehaviour,IDebugInterface
{
    ParticleSystem _particle;
    GameObject _cube;
    GameObject _cubeToggleG1;
    GameObject _cubeToggleG2;
    GameObject _cubeToggleG3;

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

    public void Integer1(int i)
    {
        Debug.Log(i);
    }


}
