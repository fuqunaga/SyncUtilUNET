﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace SyncUtil
{
    [NetworkSettings(sendInterval = 0f)]
    public class LockStep : NetworkBehaviour
    {
        // required
        public Func<MessageBase> getDataFunc;
        public Func<int, NetworkReader, bool> stepFunc;

        // optional
        public Func<bool> onMissingCatchUpServer; // if return true, StopHost() will be called.
        public Action onMissingCatchUpClient;
        public Func<string> getHashFunc; // for CheckConsistency


        public int _dataNumMax = 10000;
        public int _stepNumMaxPerFrame = 10;

        public int stepCountServer { get; protected set; }
        public int stepCountClient { get; protected set; }

        protected int _checkStepCount = -1;


        public struct Data
        {
            public int stepCount;
            public byte[] bytes;
        }

        public class SyncDatas : SyncListStruct<Data> { }

        SyncDatas _datas = new SyncDatas();



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
                var list = onMissingCatchUpServer.GetInvocationList();
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
                if (_datas.Any() && stepFunc != null)
                {
                    var currentDatas = _datas.Reverse().Where(d => d.stepCount >= stepCountClient).Reverse().Take(_stepNumMaxPerFrame);
                    if (currentDatas.Any())
                    {
                        var firstStepCount = currentDatas.First().stepCount;
                        if (firstStepCount > stepCountClient)
                        {
                            Debug.LogWarning($"Wrong step count Expected[{stepCountClient}], min data's[{firstStepCount}]");
                            onMissingCatchUpClient?.Invoke();
                        }
                        else
                        {
                            currentDatas.ToList().ForEach(data =>
                            {
                                var isStepEnable = stepFunc(data.stepCount, new NetworkReader(data.bytes));
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

                    _datas.Add(new Data() { stepCount = stepCountServer, bytes = bytes });
                    if (_datas.Count > _dataNumMax) _datas.RemoveAt(0);
                    ++stepCountServer;
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
        public ConsistencyData GetLastConsistencyData()
        {
            return _lastConsistency;
        }

        [Server]
        public void StartCheckConsistency()
        {
            StartCoroutine(CheckConsistencyCoroutine());
        }

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
            NetworkManager.singleton.client.Send(CustomMsgType.LockStepConsistency, new StringMessage(getHashFunc()));
        }
        #endregion

        #endregion
    }
}