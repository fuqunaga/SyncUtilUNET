#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Linq;

namespace SyncUtil
{
    [System.Serializable]
    public class SyncNetworkManager : NetworkManager
    {
        public static new SyncNetworkManager singleton { get { return NetworkManager.singleton as SyncNetworkManager; } }

        #region unload scene
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        static void CheckPlaymodeState()
        {
            EditorApplication.playmodeStateChanged += () =>
            {
                var startPlay = EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode;
                if (startPlay)
                {
                    var mgr = FindObjectOfType<NetworkManager>(); // singleton maybe not ready.
                    Assert.IsNotNull(mgr);

                    (new[] { mgr.onlineScene, mgr.offlineScene })
                    .Where(name => !string.IsNullOrEmpty(name))
                    .Select(name => SceneManager.GetSceneByName(name))
                    .Where(scene => scene.isLoaded)
                    .ToList()
                    .ForEach(scene => 
                    {
                        SceneManager.UnloadSceneAsync(scene);
                    });
                }
            };
        }
#endif
        #endregion


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

        public event System.Action _OnStartClient = delegate { };
        public override void OnStartClient(NetworkClient client)
        {
            base.OnStartClient(client);
            _OnStartClient();
        }

        public event System.Action<NetworkConnection> _OnClientConnect = delegate { };
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            _OnClientConnect(conn);
        }
        #endregion
    }
}