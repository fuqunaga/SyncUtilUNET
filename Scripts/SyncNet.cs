using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 0618

namespace SyncUtil
{
    public class SyncNet
    {
        /// <summary>
        /// UNETのクライアントである判定になるように偽装する
        /// あくまでSyncNetの判定のみに作用するのでちゃんとしたクライアントとしては動作せず注意が必要
        /// </summary>
        public static bool isDummyFollower { get; set; }

        public static bool isServer => NetworkServer.active && !isDummyFollower;
        public static bool isClient => NetworkClient.active || isDummyFollower;
        public static bool isHost => isServer && isClient;
        public static bool isStandAlone => !isServer && !isClient;

        public static bool isServerOrStandAlone => isServer || !isClient;

        // Not Server but Client.
        // warn Host: isServer == isClient == true
        public static bool isFollower => !isServerOrStandAlone;

        public static bool isActive { get { var nm = NetworkManager.singleton; return (nm != null) && nm.isNetworkActive; } }

        public static NetworkClient client { get { var nm = NetworkManager.singleton; return (nm == null) ? null : nm.client; } }


        public static void Spawn(GameObject go)
        {
            if (isServer) NetworkServer.Spawn(go);
        }

        public static void Destroy(GameObject go)
        {
            if (isServer) NetworkServer.Destroy(go);
            if (isServerOrStandAlone) Object.Destroy(go);
        }


        public static float time => SyncTime.Instance.time;
        public static double networkTime => SyncNetworkTime.Instance.time;
    }
}