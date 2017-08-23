using UnityEngine;
using LocalClustering;
using UnityEngine.Networking;

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
    public double time { get { return Network.time + _offset; } }

    public void Start()
    {
        var networkManager = LocalClusterNetworkManager.singleton;


        networkManager._OnServerConnect += (conn) =>
        {
            NetworkServer.SendToClient(conn.connectionId, CustomMsgType.NetworkTime, new NetworkTimeMessage() { time = Network.time });
        };


        networkManager._OnStartClient += () =>
        {
            if (LocalCluster.isSlaver)
            {
                LocalCluster.client.RegisterHandler(CustomMsgType.NetworkTime, (msg) =>
                {
                    _offset = msg.ReadMessage<NetworkTimeMessage>().time - Network.time;
                });
            }
        };
    }

    /*
    public void Update()
    {
        if ( NetworkServer.active)
        NetworkServer.SendToAll(CustomMsgType.NetworkTime, new NetworkTimeMessage() { time = Network.time });
    }
    */
}
