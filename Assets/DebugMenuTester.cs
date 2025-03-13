#if ENABLE_DEBUGTOOLKIT
using UnityEngine;

public class DebugMenuTester : MonoBehaviour
{
    void Start()
    {
        DebugViewTest debugViewTest = new DebugViewTest();
        debugViewTest.Start();
    }
}
#endif
