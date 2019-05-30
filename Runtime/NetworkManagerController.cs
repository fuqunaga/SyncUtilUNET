using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Mirror;



namespace SyncUtil
{
	[RequireComponent(typeof(SyncNetworkManager))]
    public class NetworkManagerController : MonoBehaviour
    {
        #region TypeDefine
        public enum BootType
        {
            Manual,
            Host,
            Client,
            Server
        }
        #endregion

        public virtual string _networkAddress { get; } = "localhost";
        public virtual BootType _bootType { get; } = BootType.Manual;
        public virtual bool _autoConnect { get; } = true;
        public virtual float _autoConnectInterval { get; } = 10f;


        SyncNetworkManager _networkManager;


        public virtual void Start()
        {
            _networkManager = GetComponent<SyncNetworkManager>();
			Assert.IsTrue(_networkManager);

            if (_bootType != BootType.Manual) StartNetwork(_bootType);
        }

        void StartNetwork(BootType bootType)
        {
            Assert.IsFalse(bootType == BootType.Manual);
            StopAllCoroutines();

            IEnumerator routine = null;
            var mgr = _networkManager;
            switch (bootType)
            {
                case BootType.Host: routine = StartConnectLoop(() => NetworkClient.active, () => mgr.StartHost()); break;
                case BootType.Client: routine = StartConnectLoop(() => NetworkClient.active, StartClient); break;
                case BootType.Server: routine = StartConnectLoop(() => NetworkServer.active, () => mgr.StartServer()); break;
            }

            StartCoroutine(routine);
        }

        void StartClient()
        {
            _networkManager.onClientError -= OnClientError;
            _networkManager.onClientError += OnClientError;

            _networkManager.StartClient();
        }

        void OnClientError(NetworkConnection conn, int errorCode)
        {
            _networkManager.StopClient();
        }


        IEnumerator StartConnectLoop(Func<bool> isActiveFunc, Action startFunc)
        {
            while (true)
            {
                //yield return new WaitForEndOfFrame(); // Wait for set NetworkManager callback at Start()
                yield return new WaitForSeconds(0.1f);

                UpdateManager();
                if (!isActiveFunc())
                {
                    startFunc();
                }
                yield return new WaitUntil(() => _autoConnect);
                yield return new WaitWhile(isActiveFunc);
                yield return new WaitForSeconds(_autoConnectInterval);
            }
        }

        public virtual void OnGUI()
        {
            if (_networkManager != null && !_networkManager.isNetworkActive)
            {
                GUILayout.Label("SyncUtil Manual Boot");

                GUIUtil.Indent(() =>
                {
                    OnGUINetworkSetting();

                    var mgr = _networkManager;

                    if (GUILayout.Button("Host")) { OnNetworkStartByManual(); StartNetwork(BootType.Host); }
                    if (GUILayout.Button("Client")) { OnNetworkStartByManual(); StartNetwork(BootType.Client); }
                    if (GUILayout.Button("Server")) { OnNetworkStartByManual(); StartNetwork(BootType.Server); }
                });
            }
        }

        protected virtual void OnGUINetworkSetting() { }
        protected virtual void OnNetworkStartByManual() { }



        protected void UpdateManager()
        {
            _networkManager.networkAddress = _networkAddress;
        }

        GUIUtil.Fold _fold;
        public void DebugMenu()
        {
            using (var h = new GUILayout.HorizontalScope())
            {
                GUILayout.Label("NetworkManagerController");
                GUILayout.Label(SyncNet.isHost ? "Host" : (SyncNet.isServer ? "Server" : (SyncNet.isClient ? "Client" : "StandAlone")));
                if (SyncNet.isActive)
                {
                    if (GUILayout.Button("Disconnect"))
                    {
                        NetworkManager.Shutdown();
                    }
                }
            }

            GUIUtil.Indent(() =>
            {

                DebugMenuInternal();

                if (_fold == null)
                {
                    _fold = new GUIUtil.Fold("Time Debug", () =>
                    {
                        GUILayout.Label(string.Format("SyncTime: {0:0.000}", SyncNet.time));
                        GUILayout.Label(string.Format("Network.time: {0:0.000} rtt: {1:0.000} timeSd: {2:0.000}", SyncNet.networkTime, NetworkTime.rtt, NetworkTime.timeSd));

                        NetworkServer.connections.Values.ToList().ForEach(conn => {
                            GUILayout.Label($"ConnId: {conn.connectionId}  Address: {conn.address} LastMessageTime:{conn.lastMessageTime}");
                        });
                    });
                }

                _fold.OnGUI();
            });
        }


        protected virtual void DebugMenuInternal() { }
    }
}
