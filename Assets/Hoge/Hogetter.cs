using UnityEngine;

namespace DefaultNamespace
{
    public class Hogetter : MonoBehaviour
    {
        private DebugViewHoge _hogeee =  new DebugViewHoge();

        void Start()
        {
            _hogeee.Start();
        }
    }
}