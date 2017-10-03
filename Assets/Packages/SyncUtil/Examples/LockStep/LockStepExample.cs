using UnityEngine;
using UnityEngine.Networking;

namespace SyncUtil
{
    [RequireComponent(typeof(LockStep))]
    public class LockStepExample : MonoBehaviour
    {
        public class Msg : MessageBase
        {
            public Vector3 force;
        }

        GameObject _sphere;
        Vector3 velocity;
        public float damping = 0.9f;
        public float forceMax = 0.1f;

        private void Start()
        {
            _sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _sphere.transform.SetParent(transform);

            IniteLockStepCallbacks();
        }

        void IniteLockStepCallbacks()
        {
            var lockStep = GetComponent<LockStep>();
            lockStep.getDataFunc = () =>
            {
                return new Msg()
                {
                    force = Random.insideUnitSphere * forceMax
                };
            };

            lockStep.stepFunc += (stepCount, reader) =>
            {
                var msg = reader.ReadMessage<Msg>();
                Step(msg.force);
            };

            lockStep.onMissingCatchUpServer += () =>
            {
                Debug.Log("OnMissingCatchUp at Server. NetworkManager.Shutdown() will be called.");
                return true;
            };
            lockStep.onMissingCatchUpClient += () => Debug.Log("OnMissingCatchUp at Client. Server will disconnect.");
        }


        void Step(Vector3 force)
        {
            velocity += force;
            velocity *= damping;
            var trans = _sphere.transform;
            trans.position = trans.position + velocity;
        }
    }
}
