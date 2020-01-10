using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

#pragma warning disable 0618

namespace SyncUtil
{
    public interface ILockStep
    {
        #region Required

        Func<MessageBase> getDataFunc { set; }
        Func<int, NetworkReader, bool> stepFunc { set; }

        #endregion


        #region Optional

        Func<MessageBase> getInitDataFunc { set; }
        Func<NetworkReader, bool> initFunc { set; } // if return false, skip current step and call initFunc at next frame.
        Func<bool> onMissingCatchUpServer { set; } // if return true, StopHost() will be called.
        Action onMissingCatchUpClient { set; }
        Func<string> getHashFunc { set; } // for CheckConsistency

        #endregion

        LockStep.ConsistencyData GetLastConsistencyData();
        void StartCheckConsistency();

        int stepCountServer { get; }
        int stepCountClient { get; }
    }


    [NetworkSettings(sendInterval = 0f)]
    public class LockStep : NetworkBehaviour, ILockStep
    {
        #region Overide 

        protected Func<MessageBase> _getDataFunc;
        protected Func<int, NetworkReader, bool> _stepFunc;

        protected Func<MessageBase> _getInitDataFunc;
        protected Func<NetworkReader, bool> _initFunc;

        protected Func<bool> _onMissingCatchUpServer;
        protected Action _onMissingCatchUpClient;
        protected Func<string> _getHashFunc;

        public Func<MessageBase> getDataFunc { set { _getDataFunc = value; } }
        public Func<int, NetworkReader, bool> stepFunc { set { _stepFunc = value; } }

        public Func<MessageBase> getInitDataFunc { set { _getInitDataFunc = value; } }
        public Func<NetworkReader, bool> initFunc { set { _initFunc = value; } }


        public Func<bool> onMissingCatchUpServer { set { _onMissingCatchUpServer = value; } }
        public Action onMissingCatchUpClient { set { _onMissingCatchUpClient = value; } }
        public Func<string> getHashFunc { set { _getHashFunc = value; } }

        #endregion


        #region Type Define
        public struct Data
        {
            public int stepCount;
            public byte[] bytes;
        }

        public class SyncDatas : SyncListStruct<Data> { }

        public struct InitData
        {
            public bool sended;
            public byte[] bytes;
        }

        #endregion;


        public int _dataNumMax = 10000;
        public int _stepNumMaxPerFrame = 10;

        public int stepCountServer { get; protected set; }
        public int stepCountClient { get; protected set; }

        protected int _checkStepCount = -1;



        SyncDatas _datas = new SyncDatas();

        [SyncVar]
        InitData _initData = new InitData();



        protected void Start()
        {
            var nm = SyncNetworkManager.singleton;
            nm._OnServerConnect += OnServerConnect;
        }

        protected void OnDestroy()
        {
            var nm = SyncNetworkManager.singleton;
            if (nm != null)
            {
                nm._OnServerConnect -= OnServerConnect;
            }
        }

        protected virtual void OnServerConnect(NetworkConnection conn)
        {
            var isMissingFirstData = _datas.Any() && _datas.First().stepCount > 0;
            if (isMissingFirstData)
            {
                var list = _onMissingCatchUpServer.GetInvocationList();
                var doStopHost = !list.Any() || list.Aggregate(false, (result, d) => result || ((Func<bool>)d)());
                if (doStopHost)
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
                Step();
            }
        }



        bool sendedInit;
        protected virtual void SendLockStep()
        {
            if (_getDataFunc != null)
            {
                if (!sendedInit)
                {
                    var initData = new InitData() { sended = true };

                    if (_getInitDataFunc != null)
                    {
                        var initMsg = _getInitDataFunc();
                        if (initMsg != null)
                        {
                            initData.bytes = MsgToByte(initMsg);
                        }
                    }
                    _initData = initData;
                    sendedInit = true;
                }

                var msg = _getDataFunc();
                if (msg != null)
                {
                    _datas.Add(new Data() { stepCount = stepCountServer, bytes = MsgToByte(msg) });
                    if (_datas.Count > _dataNumMax) _datas.RemoveAt(0);
                    ++stepCountServer;
                }
            }
        }


        NetworkWriter _writer = new NetworkWriter();

        byte[] MsgToByte(MessageBase msg)
        {
            _writer.SeekZero();
            _writer.Write(msg);
            return _writer.ToArray();
        }


        bool initialized;


        //static List<Data> currentDatas = new List<Data>();

        void Step()
        {
            if (!_datas.Any() || _stepFunc == null) return;

            var idx = _datas.Count-1;
            for (; idx>=0; --idx)
            {
                var step = _datas[idx].stepCount;
                if ( step == stepCountClient) break;
                if (step < stepCountClient) return;
            }

            if ( idx>=0 )
            { 
                var firstStepCount = _datas[idx].stepCount;
                if (firstStepCount > stepCountClient)
                {
                    Debug.LogWarning($"Wrong step count Expected[{stepCountClient}], min data's[{firstStepCount}]");
                    _onMissingCatchUpClient?.Invoke();
                }
                else
                {
                    if (!initialized)
                    {
                        if (_initFunc != null)
                        {
                            if (!_initData.sended)
                            {
                                // InitData is NOT reach to this client yet
                                return;
                            }
                            initialized = _initFunc(new NetworkReader(_initData.bytes));
                        }
                        else
                        {
                            initialized = true;
                        }
                    }

                    if (initialized)
                    {
                        var limit = Mathf.Min(idx + _stepNumMaxPerFrame, _datas.Count);
                        for (; idx < limit; idx++)
                        {
                            var data = _datas[idx];
                            Assert.IsTrue(stepCountClient == data.stepCount, $"stepCountClient[{stepCountClient}] data.stepCount[{data.stepCount}]");
                            
                            var isStepEnable = _stepFunc(data.stepCount, new NetworkReader(data.bytes));
                            if (isStepEnable)
                            {
                                if (_checkStepCount >= 0)
                                {
                                    Assert.IsTrue(stepCountClient <= _checkStepCount);
                                    if (stepCountClient == _checkStepCount)
                                    {
                                        ReturnCheckConsistency();
                                        _checkStepCount = -1;
                                    }
                                }
                                ++stepCountClient;
                            }
                        }
                    }
                }
            }
        }


        #region Check Consistency Implement

        #region server

        public Dictionary<int, string> connectionIdToHash { get; protected set; } = new Dictionary<int, string>();
        protected bool isCompleteConnectionIdToHash => connectionIdToHash.Count == NetworkServer.connections.Count;

        public enum Consistency
        {
            NOT_CHECK_YET,
            CHECKING,
            MATCH,
            NOT_MATCH,
            TIME_OUT
        }

        public class ConsistencyData
        {
            public int stepCount;
            public Consistency consistency;
        }

        ConsistencyData _lastConsistency = new ConsistencyData()
        {
            stepCount = -1,
            consistency = Consistency.NOT_CHECK_YET
        };


        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.RegisterHandler(CustomMsgType.LockStepConsistency, (nmsg) =>
            {
                connectionIdToHash[nmsg.conn.connectionId] = nmsg.ReadMessage<StringMessage>().value;
            });
        }


        [Server]
        public ConsistencyData GetLastConsistencyData() => _lastConsistency;

        [Server]
        public void StartCheckConsistency() => StartCoroutine(CheckConsistencyCoroutine());

        protected IEnumerator CheckConsistencyCoroutine(float timeOut = 10f, int delayStepCount = 10)
        {
            connectionIdToHash.Clear();

            var checkStepCount = stepCountServer + delayStepCount;
            _lastConsistency.stepCount = checkStepCount;
            _lastConsistency.consistency = Consistency.CHECKING;

            NetworkServer.SendToAll(CustomMsgType.LockStepConsistency, new IntegerMessage(checkStepCount));
            var time = Time.time;

            yield return new WaitUntil(() => ((Time.time - time) > timeOut) || isCompleteConnectionIdToHash);


            if (isCompleteConnectionIdToHash)
            {
                _lastConsistency.consistency = (connectionIdToHash.Values.Distinct().Count() == 1) ? Consistency.MATCH : Consistency.NOT_MATCH;
            }
            else
            {
                _lastConsistency.consistency = Consistency.TIME_OUT;
            }
        }
        #endregion


        #region client
        public override void OnStartClient()
        {
            base.OnStartClient();
            NetworkManager.singleton.client.RegisterHandler(CustomMsgType.LockStepConsistency, (nmsg) =>
            {
                _checkStepCount = nmsg.ReadMessage<IntegerMessage>().value;
            });
        }


        [Client]
        protected void ReturnCheckConsistency()
        {
            NetworkManager.singleton.client.Send(CustomMsgType.LockStepConsistency, new StringMessage(_getHashFunc()));
        }
        #endregion

        #endregion
    }
}