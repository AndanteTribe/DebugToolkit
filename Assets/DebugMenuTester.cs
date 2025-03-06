using UnityEngine;
using UnityEngine.UIElements;
using DebugToolkit;
public class DebugMenuTester : MonoBehaviour
{
    void Start()
    {
        DebugViewTest debugViewTest = new DebugViewTest();
        debugViewTest.Start();
    }
}
