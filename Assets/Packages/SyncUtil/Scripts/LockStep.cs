using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace SyncUtil
{
    [NetworkSettings(sendInterval = 0f)]
    public class LockStep : NetworkBehaviour
    {
        public Func<MessageBase> getDataFunc;
        public Action<int, NetworkReader> stepFunc;
        public Func<bool> onMissingCatchUpServer; // if return true, StopHost() will be called.
        public Action onMissingCatchUpClient;


        public int _dataNumMax = 10000;
        public int _stepNumMaxPerFrame = 10;

        protected int _stepCountServer;
        protected int _stepCountClient;

        public struct Data
        {
            public int stepCount;
            public byte[] bytes;
        }

        public class SyncDatas : SyncListStruct<Data> { }

        SyncDatas _datas = new SyncDatas();


        private void Start()
        {
            SyncNetworkManager.singleton._OnServerConnect += OnServerConnect;
        }

        private void OnDestroy()
        {
            var mgr = SyncNetworkManager.singleton;
            if (mgr != null)
            {
                mgr._OnServerConnect -= OnServerConnect;
            }
        }

        protected virtual void OnServerConnect(NetworkConnection conn)
        {
            if (_datas.Any() && _datas.First().stepCount > 0)
            {
                var list = onMissingCatchUpServer.GetInvocationList();
                var doStopHost = !list.Any() || list.Aggregate(false, (result, d) => result || ((Func<bool>)d)());
                if ( doStopHost)
                {
                    NetworkManager.singleton.StopHost();
                }
            }
        }


        protected virtual void Update()
        {
            if (SyncNet.isServer)
            {
                SendLockStep();
            }

            if (SyncNet.isClient)
            {
                if (_datas.Any() && stepFunc != null)
                {
                    var currentDatas = _datas.Reverse().Where(d => d.stepCount >= _stepCountClient).Reverse().Take(_stepNumMaxPerFrame);
                    if (currentDatas.Any())
                    {
                        var firstStepCount = currentDatas.First().stepCount;
                        if (firstStepCount > _stepCountClient)
                        {
                            Debug.LogWarning($"Wrong step count Expected[{_stepCountClient}], min data's[{firstStepCount}]");
                            onMissingCatchUpClient?.Invoke();
                        }
                        else
                        {
                            currentDatas.ToList().ForEach(data =>
                            {
                                stepFunc(data.stepCount, new NetworkReader(data.bytes));
                                ++_stepCountClient;
                            });
                        }
                    }
                }
            }
        }

        NetworkWriter _writer = new NetworkWriter();
        protected virtual void SendLockStep()
        {
            if (getDataFunc != null)
            {
                var msg = getDataFunc();
                if (msg != null)
                {
                    _writer.SeekZero();
                    _writer.Write(msg);
                    var bytes = _writer.ToArray();

                    _datas.Add(new Data() { stepCount = _stepCountServer, bytes = bytes });
                    if (_datas.Count > _dataNumMax) _datas.RemoveAt(0);
                    ++_stepCountServer;
                }
            }
        }
    }
}