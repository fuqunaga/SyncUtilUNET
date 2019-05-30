using Mirror;
using UnityEngine;
using UnityEngine.Assertions;

namespace SyncUtil
{
    [System.Serializable]
    public class SyncNetworkManager : NetworkManager
    {
        public static new SyncNetworkManager singleton => NetworkManager.singleton as SyncNetworkManager;

        public override void Awake()
        {
            base.Awake();

            Assert.IsNotNull(playerPrefab, "playerPrefab == null. Mirror will not spawns scene objects.");
            if ( !autoCreatePlayer)
            {
                Debug.LogWarning("autoCreatePlayer == false. Mirror spawns scene objects at AddPlayer. you must call ClientScene.AddPlayer() manually.");
            }
        }

        #region Server side

        public event System.Action onStartServer = delegate { };
        public override void OnStartServer()
        {
            base.OnStartServer();
            onStartServer();
        }

        public event System.Action<NetworkConnection> onServerConnect = delegate { };
        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            onServerConnect(conn);
        }

        public event System.Action<NetworkConnection> onServerDisconnect = delegate { };
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            onServerDisconnect(conn);
        }

        #endregion


        #region Client side

        public event System.Action onStartClient = delegate { };
        public override void OnStartClient()
        {
            base.OnStartClient();
            onStartClient();
        }

        public event System.Action<NetworkConnection> onClientConnect = delegate { };
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            onClientConnect(conn);
        }


        public event System.Action<NetworkConnection, int> onClientError = delegate { };
        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            base.OnClientError(conn, errorCode);
            onClientError(conn, errorCode);
        }
        #endregion
    }
}