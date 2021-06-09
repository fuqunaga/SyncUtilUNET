using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 0618

namespace SyncUtil
{
    public class SyncNetworkTime : MonoBehaviour
    {
        #region singleton
        protected static SyncNetworkTime _instance;
        public static SyncNetworkTime Instance { get { return _instance ?? (_instance = (FindObjectOfType<SyncNetworkTime>() ?? (new GameObject("SyncNetworkTime", typeof(SyncNetworkTime))).GetComponent<SyncNetworkTime>())); } }
        #endregion

        #region type define
        public class NetworkTimeMessage : MessageBase
        {
            public double time;
        }
        #endregion

        double _offset;

        protected float realTime => Time.realtimeSinceStartup;

        public double time => realTime + _offset;

        public void Start()
        {
            var networkManager = SyncNetworkManager.singleton;


            networkManager.onServerConnect += (conn) =>
            {
                NetworkServer.SendToClient(conn.connectionId, CustomMsgType.NetworkTime, new NetworkTimeMessage() { time = realTime });
            };


            networkManager.onStartClient += (client) =>
            {
                if (SyncNet.isFollower)
                {
                    client.RegisterHandler(CustomMsgType.NetworkTime, (msg) =>
                    {
                        _offset = msg.ReadMessage<NetworkTimeMessage>().time - realTime;
                    });
                }
            };
        }
    }
}