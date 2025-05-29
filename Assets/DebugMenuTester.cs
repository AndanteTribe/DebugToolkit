using UnityEngine;
using UnityEngine.InputSystem;

namespace DebugToolkit
{
    public class DebugMenuTester : MonoBehaviour
    {
        void Start()
        {
            DebugViewTest debugViewTest = new DebugViewTest();
            debugViewTest.Start();
        }

        private void Update()
        {
            // Debug.Log(Pointer.current.position.ReadValue());
        }
    }
}