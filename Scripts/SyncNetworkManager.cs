using Mirror;


namespace SyncUtil
{
    [System.Serializable]
    public class SyncNetworkManager : NetworkManager
    {
        public static new SyncNetworkManager singleton => NetworkManager.singleton as SyncNetworkManager;


        #region Server side

        public event System.Action _OnStartServer = delegate { };
        public override void OnStartServer()
        {
            base.OnStartServer();
            _OnStartServer();
        }

        public event System.Action<NetworkConnection> _OnServerConnect = delegate { };
        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            _OnServerConnect(conn);
        }

        public event System.Action<NetworkConnection> _OnServerDisconnect = delegate { };
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            _OnServerDisconnect(conn);
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