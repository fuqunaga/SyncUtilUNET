using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;


namespace LocalClustering
{
    [RequireComponent(typeof(NetworkManager))]
    [RequireComponent(typeof(LatencyChecker), typeof(SyncTimeManager), typeof(SyncNetworkTime))]
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
        public virtual int _networkPort { get; } = 7777;
        public virtual BootType _bootType { get; } = BootType.Manual;
        public virtual bool _autoConnect { get; } = true;
        public virtual float _autoConnectInterval { get; } = 10f;


        public void Start()
        {
            UpdateManager();

            if (_bootType != BootType.Manual) StartNetwork(_bootType);
        }

        void StartNetwork(BootType bootType)
        {
            Assert.IsFalse(bootType == BootType.Manual);
            StopAllCoroutines();

            IEnumerator routine = null;
            var mgr = NetworkManager.singleton;
            switch (bootType)
            {
                case BootType.Host: routine = StartConnectLoop(() => mgr.client != null, () => mgr.StartHost()); break;
                case BootType.Client: routine = StartConnectLoop(() => mgr.client != null, () => mgr.StartClient()); break;
                case BootType.Server: routine = StartConnectLoop(() => NetworkServer.active, () => mgr.StartServer()); break;
            }

            StartCoroutine(routine);
        }

        IEnumerator StartConnectLoop(Func<bool> isActiveFunc, Action startFunc)
        {
            while (true)
            {
                yield return new WaitForEndOfFrame(); // Wait for set NetworkManager callback at Start()
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
            var mgr = NetworkManager.singleton;
            if (mgr != null && !mgr.isNetworkActive)
            {
                GUILayout.Label("LocalCluster Manual Boot");

                GUIUtil.Indent(() =>
                {
                    /*
                    var mgr = NetworkManager.singleton;
                    mgr.useSimulator = GUILayout.Toggle(mgr.useSimulator, "UseSimulator");
                    if (mgr.useSimulator)
                    {
                        GUIUtil.Indent(() =>
                        {
                            mgr.simulatedLatency = GUIUtil.Slider(mgr.simulatedLatency, 1, 400, "Latency[msec]");
                            mgr.packetLossPercentage = GUIUtil.Slider(mgr.packetLossPercentage, 0f, 20f, "PacketLoss[%]");
                        });
                    }
                    */

                    if (GUILayout.Button("Host")) { OnNetworkStartByManual(); StartNetwork(BootType.Host); }
                    if (GUILayout.Button("Client")) { OnNetworkStartByManual(); StartNetwork(BootType.Client); }
                    if (GUILayout.Button("Server")) { OnNetworkStartByManual(); StartNetwork(BootType.Server); }
                });
            }
        }

        protected virtual void OnNetworkStartByManual() { }



        void UpdateManager()
        {
            var mgr = NetworkManager.singleton;
            mgr.networkAddress = _networkAddress;
            mgr.networkPort = _networkPort;
        }

        GUIUtil.Fold _fold;
        public void DebugMenu()
        {
            using (var h = new GUILayout.HorizontalScope())
            {
                GUILayout.Label("NetworkManagerController");
                GUILayout.Label(LocalCluster.isHost ? "Host" : (LocalCluster.isServer ? "Server" : (LocalCluster.isClient ? "Client" : "StandAlone")));
                if (LocalCluster.isActive)
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
                        GUILayout.Label(string.Format("SyncTime: {0:0.000}", LocalCluster.time));
                        GUILayout.Label(string.Format("Network.time Synced/Orig: {0:0.000} / {1:0.000}", LocalCluster.networkTime, Network.time));

                        LatencyChecker.Instance._conectionLatencyTable.ToList().ForEach(pair =>
                        {
                            var data = pair.Value;
                            GUILayout.Label(string.Format("ConnId: {0}  Latency: {1:0.000} Average:{2:0.000} " + (data._recieved ? "✔" : ""), pair.Key, data.Last, data.average));
                        });
                    });
                }

                _fold.OnGUI();
            });
        }


        protected virtual void DebugMenuInternal() { }
    }
}
