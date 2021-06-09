using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable CS0618

namespace SyncUtil
{
    /// <summary>
    /// ClientのConnectionに任意の名前をつけ、サーバー側で区別できるようにする
    /// Client側からの自己申告性
    /// 区別するにはClientごとにNameの値を変える必要があるので注意
    /// </summary>
    public class ClientNameManager : ClientNameManagerBase
    {
        public string myName;
        protected override string Name => myName;
    }


    public abstract class ClientNameManagerBase : MonoBehaviour
    {
        #region Type Define

        public class Message : MessageBase
        {
            public string name;
        }

        #endregion


        #region Static

        static ClientNameManagerBase instance;

        public static ClientNameManagerBase Instance => instance != null ? instance : instance = FindObjectOfType<ClientNameManagerBase>();

        #endregion


        Dictionary<NetworkConnection, string> nameDic = new Dictionary<NetworkConnection, string>();

        protected abstract string Name { get; }

        protected virtual void Start()
        {
            if (SyncNet.isServer)
            {
                NetworkServer.RegisterHandler(CustomMsgType.ConnectionIdentity, OnReceiveConnectionIdentity);

                var manager = SyncNetworkManager.singleton;
                manager.onServerDisconnect += (conn) => nameDic.Remove(conn);
            }

            if (SyncNet.isClient)
            {
                var client = SyncNet.client;

                var message = new Message() { name = Name };
                client.Send(CustomMsgType.ConnectionIdentity, message);
            }
        }


        #region Server


        void OnReceiveConnectionIdentity(NetworkMessage netMsg)
        {
            var msg = netMsg.ReadMessage<Message>();
            nameDic[netMsg.conn] = msg.name;
        }

        public string GetClientName(NetworkConnection conn)
        {
            nameDic.TryGetValue(conn, out var ret);
            return ret;
        }

        #endregion
    }
}