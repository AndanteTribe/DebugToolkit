using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DebugToolkit
{
    public class DebugMenuTester : MonoBehaviour
    {
        public ParticleSystem particle;
        public GameObject cube;
        public GameObject[] toggleObjects=new GameObject[3];
        public Slider slider;
        public Text text;
        public Slider vSlider;
        public ParticleSystem emitter;
        public RandomSpawner randomSpawner;
        public Text dropFieldText;
        public Text enumFieldText;
        public Material material;
        public GameObject hideObject;
        void Start()
        {
            DebugViewTest debugViewTest = new DebugViewTest(particle,cube,
                toggleObjects[0],toggleObjects[1],toggleObjects[2],
                slider,text,vSlider,emitter,randomSpawner,
                dropFieldText,enumFieldText,hideObject,material);
            debugViewTest.Start();
        }

        private void Update()
        {
            Debug.Log(Pointer.current.position.ReadValue());
        }
    }
}