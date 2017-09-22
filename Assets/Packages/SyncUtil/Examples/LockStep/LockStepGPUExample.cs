using UnityEngine;
using UnityEngine.Networking;

namespace SyncUtil.Example
{
    [RequireComponent(typeof(LockStep), typeof(GPUFluid), typeof(GPUFluidMouse))]
    public class LockStepGPUExample : MonoBehaviour
    {
        public class Msg : MessageBase
        {
            public GPUFluid.StepData data;
        }

        GPUFluid _fluid;
        GPUFluidMouse _mouse;

        private void Start()
        {
            _fluid = GetComponent<GPUFluid>();
            _mouse = GetComponent<GPUFluidMouse>();

            IniteLockStepCallbacks();
        }

        void IniteLockStepCallbacks()
        {
            var lockStep = GetComponent<LockStep>();
            lockStep.getDataFunc = () =>
            {
                return new Msg()
                {
                    data = GPUFluidUpdator.CreateStepData(_mouse)
                };
            };

            lockStep.stepFunc += (stepCount, reader) =>
            {
                var msg = reader.ReadMessage<Msg>();
                Step(msg.data);
            };

            lockStep.onMissingCatchUpServer += () =>
            {
                Debug.Log("OnMissingCatchUp at Server. NetworkManager.Shutdown() will be called.");
                NetworkManager.Shutdown();
            };
            lockStep.onMissingCatchUpClient += () => Debug.Log("OnMissingCatchUp at Client. Server will disconnect.");
        }


        void Step(GPUFluid.StepData data)
        {
            _fluid.Step(data);
        }
    }
}
