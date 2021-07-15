using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 0618

namespace SyncUtil.Example
{
    [RequireComponent(typeof(LockStep), typeof(LifeGame))]
    public class LockStepGPUExample : LockStepExampleBase
    {
        public class Msg : MessageBase
        {
            public LifeGame.StepData data;
        }

        public float _resolutionScale = 0.5f;
        LifeGame _lifeGame;

        protected override void Start()
        {
            base.Start();
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

            lockStep.stepFunc = (stepCount, reader) =>
            {
                if (_stepEnable)
                {
                    var msg = reader.ReadMessage<Msg>();
                    _lifeGame.Step(msg.data);
                }
                return _stepEnable;
            };

            lockStep.onMissingCatchUpServer = () =>
            {
                Debug.Log("OnMissingCatchUp at Server. NetworkManager.StopHost() will be called.");
                return true;
            };
            lockStep.onMissingCatchUpClient = () => Debug.Log("OnMissingCatchUp at Client. Server will disconnect.");

            lockStep.getHashFunc = () => LockStepHelper.GenerateBufferHash<LifeGame.Data>(_lifeGame.readBufs);
        }
    }
}
