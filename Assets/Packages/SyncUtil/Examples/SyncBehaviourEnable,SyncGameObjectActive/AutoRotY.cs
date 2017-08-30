using UnityEngine;

namespace SyncUtil
{
    public class AutoRotY : MonoBehaviour
    {
        public float _speed = 10f;

        void Update()
        {
            transform.Rotate(Vector3.up, _speed * Time.deltaTime);
        }
    }
}