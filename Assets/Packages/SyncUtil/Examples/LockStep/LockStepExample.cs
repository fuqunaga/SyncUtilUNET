using UnityEngine;
using UnityEngine.Networking;

namespace SyncUtil.Example
{
    [RequireComponent(typeof(LockStep))]
    public class LockStepExample : LockStepExampleBase
    {
        public class Msg : MessageBase
        {
            public Vector3 force;
        }

        GameObject _sphere;
        Vector3 velocity;
        public float damping = 0.9f;
        public float forceMax = 0.1f;

        protected override void Start()
        {
            base.Start();

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
                if (_stepEnable)
                {
                    var msg = reader.ReadMessage<Msg>();
                    Step(msg.force);
                }
                return _stepEnable;
            };

            lockStep.onMissingCatchUpServer += () =>
            {
                Debug.Log("OnMissingCatchUp at Server. NetworkManager.Shutdown() will be called.");
                return true;
            };
            lockStep.onMissingCatchUpClient += () => Debug.Log("OnMissingCatchUp at Client. Server will disconnect.");

            lockStep.getHashFunc += () =>
            {
                return _sphere.transform.position.ToString(".00000");
            };
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
