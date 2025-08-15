using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DebugToolkit
{
    public class DebugMenuTester : MonoBehaviour
    {
        public ParticleSystem _particle;
        public GameObject _cube;
        public GameObject[] _toggleObjects = new GameObject[3];
        public Slider _slider;
        public Text _text;
        public Slider _vSlider;
        public ParticleSystem _emitter;
        public RandomSpawner _randomSpawner;
        public Text _dropFieldText;
        public Text _enumFieldText;
        public Material _material;
        public GameObject _hideObject;
        void Start()
        {
            DebugViewTest debugViewTest = new DebugViewTest(_particle,_cube,
                _toggleObjects[0],_toggleObjects[1],_toggleObjects[2],
                _slider,_text,_vSlider,_emitter,_randomSpawner,
                _dropFieldText,_enumFieldText,_hideObject,_material);
            debugViewTest.Start();
        }

        private void Update()
        {
            Debug.Log(Pointer.current.position.ReadValue());
        }
    }
}