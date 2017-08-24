#define INCLUDE_UPDATE

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace SyncUtil
{

    /// <summary>
    /// 以下の時間を計測し、この半分をServerの値がClientに反映される時間（Latency）として計測する
    /// 
    /// INCLUDE_UPDATE defined
    /// (Server)Send -> (Client)Recieve -> (Client)Update,Send -> (Server)Recieve -> (Server)Update
    /// 
    /// INCLUDE_UPATE not defined
    /// (Server)Send -> (Client)Recieve,Send -> (Server)Recieve
    /// </summary>
    public class LatencyChecker : MonoBehaviour
    {
        #region singleton
        protected static LatencyChecker _instance;
        public static LatencyChecker Instance { get { return _instance ?? (_instance = (FindObjectOfType<LatencyChecker>() ?? (new GameObject("LatencyChecker", typeof(LatencyChecker))).GetComponent<LatencyChecker>())); } }
        #endregion

        #region type define
        public class LatencyMessage : MessageBase
        {
            public double serverTime;
        }

        public class Data
        {
            static int _queueNum = 8;
            Queue<float> _latencies = new Queue<float>();
            public bool _recieved;

            public void Add(float latency)
            {
                _latencies.Enqueue(latency);
                while (_latencies.Count > _queueNum) _latencies.Dequeue();
            }

            public float Last { get { return _latencies.LastOrDefault(); } }
            public float average { get { return _latencies.Average(); } }
        }
        #endregion


        public Dictionary<int, Data> _conectionLatencyTable = new Dictionary<int, Data>();

#if INCLUDE_UPDATE
        Dictionary<int, LatencyMessage> _conectionLatencyPool = new Dictionary<int, LatencyMessage>();
        LatencyMessage _lastMsg;
#endif

        public void Start()
        {
            SyncNetworkManager.singleton._OnStartServer += () =>
            {
                NetworkServer.RegisterHandler(CustomMsgType.Latency, (nmsg) =>
                {
#if INCLUDE_UPDATE
                    _conectionLatencyPool[nmsg.conn.connectionId] = nmsg.ReadMessage<LatencyMessage>();
#else
                    UpdateTable(nmsg.conn.connectionId, nmsg.ReadMessage<LatencyMessage>());
#endif
                });
            };

            SyncNetworkManager.singleton._OnServerDisconnect += (conn) =>
            {
                _conectionLatencyTable.Remove(conn.connectionId);
            };

            SyncNetworkManager.singleton._OnStartClient += () =>
            {
                if (SyncNet.isSlaver)
                {
                    SyncNet.client.RegisterHandler(CustomMsgType.Latency, (msg) =>
                    {
#if INCLUDE_UPDATE
                        _lastMsg = msg.ReadMessage<LatencyMessage>();
#else
                        LocalCluster.client.Send(CustomMsgType.Latency, msg.ReadMessage<LatencyMessage>());
#endif
                    });

                }
            };
        }

        public void Update()
        {
            if (SyncNet.isServer)
            {
                _conectionLatencyTable.Values.ToList().ForEach(d => d._recieved = false);
                NetworkServer.SendToAll(CustomMsgType.Latency, new LatencyMessage() { serverTime = Network.time });


#if INCLUDE_UPDATE
                if (_conectionLatencyPool.Any())
                {
                    _conectionLatencyPool.ToList().ForEach(pair =>
                    {
                        UpdateTable(pair.Key, pair.Value);
                    });
                    _conectionLatencyPool.Clear();
                }
#endif
            }

#if INCLUDE_UPDATE
            if (SyncNet.isSlaver)
            {
                if (_lastMsg != null)
                {
                    SyncNet.client.Send(CustomMsgType.Latency, _lastMsg);
                    _lastMsg = null;
                }
            }
#endif      
        }

        void UpdateTable(int connectionId, LatencyMessage lmsg)
        {
            Data data;
            if (!_conectionLatencyTable.TryGetValue(connectionId, out data))
            {
                _conectionLatencyTable[connectionId] = data = new Data();
            }

            var latency = (float)(Network.time - lmsg.serverTime) * 0.5f;
            data.Add(latency);
            data._recieved = true;
        }
    }
}