using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace LocalClustering
{
    public class SyncTimeManager : MonoBehaviour
    {
        #region singleton
        protected static SyncTimeManager _instance;
        public static SyncTimeManager Instance { get { return _instance ?? (_instance = (FindObjectOfType<SyncTimeManager>() ?? (new GameObject("SyncTimeManager", typeof(SyncTimeManager))).GetComponent<SyncTimeManager>())); } }
        #endregion

        #region type define
        public class SyncTimeMessage : MessageBase
        {
            public float time;
            public float timeScale;
        }
        #endregion

        public float time
        {
            get { return LocalCluster.isServerOrStandAlone ? Time.time : _clientTime; }
        }


        float _clientTime;
        SyncTimeMessage _lastMsg;

        public void Start()
        {
            LocalClusterNetworkManager.singleton._OnStartClient += () =>
            {
                if (LocalCluster.isSlaver)
                {
                    LocalCluster.client.RegisterHandler(CustomMsgType.Time, (netMsg) =>
                    {
                        var msg = netMsg.ReadMessage<SyncTimeMessage>();
                        if (_lastMsg == null || msg.time > _lastMsg.time)
                        {
                            _lastMsg = msg;
                        }
                    });

                    StopAllCoroutines();
                    StartCoroutine(UpdateTimeClient());
                }
            };
        }

        public void Update()
        {
            if (LocalCluster.isServer)
            {
                NetworkServer.SendToAll(CustomMsgType.Time, new SyncTimeMessage() { time = time, timeScale = Time.timeScale });
            }
        }

        IEnumerator UpdateTimeClient()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();  // フレームの最後で_time更新

                if (_lastMsg != null)
                {
                    _clientTime = _lastMsg.time;
                    Time.timeScale = _lastMsg.timeScale;
                }
            }
        }
    }
}