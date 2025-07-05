using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectFinderScript : MonoBehaviour
{
    public void StartObjectFinder(bool isRun)
    {
        Debug.Log("check findeer");
        if (isRun) return;
        isRun = true;
        Debug.Log("start");
        StartCoroutine("ObjectFinder");
    }

    IEnumerator ObjectFinder()
    {
        while (true)
        {
            yield return null;
            Debug.Log("finding");
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape))
            {

            }
        }
    }
}
