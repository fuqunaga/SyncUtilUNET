using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace LocalClustering
{
    public class NetworkSpawner : MonoBehaviour
    {
        public List<NetworkIdentity> _prefabs = new List<NetworkIdentity>();

        void Start()
        {
            LocalClusterNetworkManager.singleton._OnStartServer += () =>
            {
                StartCoroutine(DelaySpawn());
            };
        }

        IEnumerator DelaySpawn()
        {
            // _OnStartServer() 呼び出し時はまだNetworkServer.activeではないことがある
            yield return new WaitUntil(() => NetworkServer.active);

            _prefabs
            .Select(p => Instantiate(p.gameObject))
            .ToList()
            .ForEach(go =>
            {
                go.transform.SetParent(transform);
                NetworkServer.Spawn(go);
            });
        }
    }
}