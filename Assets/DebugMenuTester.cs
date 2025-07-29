using UnityEngine;
using UnityEngine.InputSystem;

namespace DebugToolkit
{
    public class DebugMenuTester : MonoBehaviour
    {
        public ParticleSystem particle;
        public GameObject cube;
        public GameObject[] toggleObjects=new GameObject[3];
        void Start()
        {
            DebugViewTest debugViewTest = new DebugViewTest(particle,cube,
                toggleObjects[0],toggleObjects[1],toggleObjects[2]);
            debugViewTest.Start();
        }

        private void Update()
        {
            Debug.Log(Pointer.current.position.ReadValue());
        }
    }
}