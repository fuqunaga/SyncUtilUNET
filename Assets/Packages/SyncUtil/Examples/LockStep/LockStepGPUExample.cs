using UnityEngine;
using UnityEngine.Networking;

namespace SyncUtil.Example
{
    [RequireComponent(typeof(LockStep), typeof(LifeGame))]
    public class LockStepGPUExample : MonoBehaviour
    {
        public class Msg : MessageBase
        {
            public LifeGame.StepData data;
        }

        public float _resolutionScale = 0.5f;
        LifeGame _lifeGame;

        private void Start()
        {
            _lifeGame = GetComponent<LifeGame>();
            LifeGameUpdater.Reset();

            InitLockStepCallbacks();
        }

        void InitLockStepCallbacks()
        {
            var lockStep = GetComponent<LockStep>();
            lockStep.getDataFunc = () =>
            {
                return new Msg()
                {
                    data = LifeGameUpdater.CreateStepData(_resolutionScale)
                };
            };

            lockStep.stepFunc += (stepCount, reader) =>
            {
                var msg = reader.ReadMessage<Msg>();
                _lifeGame.Step(msg.data);
            };

            lockStep.onMissingCatchUpServer += () =>
            {
                Debug.Log("OnMissingCatchUp at Server. NetworkManager.StopHost() will be called.");
                NetworkManager.singleton.StopHost();
            };
            lockStep.onMissingCatchUpClient += () => Debug.Log("OnMissingCatchUp at Client. Server will disconnect.");
        }
    }
}
