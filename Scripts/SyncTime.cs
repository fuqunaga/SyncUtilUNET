using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 0618

namespace SyncUtil
{
    public class SyncTime : MonoBehaviour
    {
        #region singleton
        protected static SyncTime _instance;
        public static SyncTime Instance { get { return _instance ?? (_instance = (FindObjectOfType<SyncTime>() ?? (new GameObject("SyncTimeManager", typeof(SyncTime))).GetComponent<SyncTime>())); } }
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
            get { return SyncNet.isServerOrStandAlone ? Time.time : _clientTime; }
        }


        float _clientTime;
        SyncTimeMessage _lastMsg;

        public void Start()
        {
			if(SyncNetworkManager.singleton == null)
			{
				Debug.LogWarning("SyncNetworkManager is not in scene");
				return;
			}
            SyncNetworkManager.singleton._OnStartClient += (client) =>
            {
                if (SyncNet.isSlave)
                {
                    client.RegisterHandler(CustomMsgType.Time, (netMsg) =>
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
            if (SyncNet.isServer)
            {
                NetworkServer.SendToAll(CustomMsgType.Time, new SyncTimeMessage() { time = time, timeScale = Time.timeScale });
            }
        }

       WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();  // フレームの最後で_time更新

        IEnumerator UpdateTimeClient()
        {
            while (true)
            {
                yield return waitForEndOfFrame;  // フレームの最後で_time更新

                if (_lastMsg != null)
                {
                    _clientTime = _lastMsg.time;
                    Time.timeScale = _lastMsg.timeScale;
                }
            }
        }
    }
}