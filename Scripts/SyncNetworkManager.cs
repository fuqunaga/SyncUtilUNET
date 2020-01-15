using UnityEngine.Networking;
using UnityEngine;

#pragma warning disable 0618

namespace SyncUtil
{
    [System.Serializable]

    public class SyncNetworkManager : NetworkManager
    {
        public static new SyncNetworkManager singleton { get { return NetworkManager.singleton as SyncNetworkManager; } }


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

        public event System.Action<NetworkClient> _OnStartClient = delegate { };
        public override void OnStartClient(NetworkClient client)
        {
			Debug.LogFormat("{0} Client networking logic is starting", System.DateTime.Now);
            base.OnStartClient(client);
            _OnStartClient(client);
        }

        public event System.Action<NetworkConnection> _OnClientConnect = delegate { };
        public override void OnClientConnect(NetworkConnection conn)
        {
			Debug.LogFormat("{0} Client connection ID: {1}  has connected to the server", System.DateTime.Now, conn.connectionId);
            base.OnClientConnect(conn);
            _OnClientConnect(conn);
        }


        public event System.Action<NetworkConnection, int> _OnClientError = delegate { };
        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
			Debug.LogErrorFormat("{0} Client Error: connection ID: {1}  error code: {2} error message: {3} ",System.DateTime.Now, conn.connectionId, errorCode, NetworkErrorToString(errorCode));
            base.OnClientError(conn, errorCode);
            _OnClientError(conn, errorCode);
        }

		public override void OnClientDisconnect(NetworkConnection conn)
		{
			Debug.LogWarningFormat("{0} Client Disconeect  connection ID {1}", System.DateTime.Now, conn.connectionId);
			base.OnClientDisconnect(conn);
		}
		#endregion

		string NetworkErrorToString(int networkErrorID)
		{
			switch ((NetworkError)networkErrorID)
			{
				case NetworkError.Ok:					return "OK. no error!";
				case NetworkError.WrongHost:			return "wrong host";
				case NetworkError.WrongConnection:		return "wrong connection";
				case NetworkError.WrongChannel:			return "wrong Channel";
				case NetworkError.NoResources:			return "no resources";
				case NetworkError.BadMessage:			return "bad message";
				case NetworkError.Timeout:				return "timeout";
				case NetworkError.MessageToLong:		return "message to long";
				case NetworkError.WrongOperation:		return "wrong operation";
				case NetworkError.VersionMismatch:		return "version mismatch";
				case NetworkError.CRCMismatch:			return "CRC mismatch";
				case NetworkError.DNSFailure:			return "DNS failure";
				case NetworkError.UsageError:			return "usage error";
				default:								return string.Format("unknown error code ({0})", networkErrorID);
			}
		
		}
	}
}