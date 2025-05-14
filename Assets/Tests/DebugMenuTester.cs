using UnityEngine;

namespace DebugToolkit.Tests
{
    public class DebugMenuTester : MonoBehaviour
    {
        void Start()
        {
            DebugViewTest debugViewTest = new DebugViewTest();
            debugViewTest.Start();
        }
    }
}