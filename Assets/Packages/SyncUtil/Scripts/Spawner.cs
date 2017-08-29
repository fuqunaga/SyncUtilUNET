using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace SyncUtil
{
    [ExecuteInEditMode]
    public class Spawner : MonoBehaviour
    {
        public List<NetworkIdentity> _prefabs = new List<NetworkIdentity>();

#if UNITY_EDITOR
        NetworkManager _networkManager;
        private void Update()
        {
            var nm = _networkManager ?? (_networkManager = FindObjectOfType<NetworkManager>());
            var diffGo = _prefabs.Select(ni => ni.gameObject).Except(nm.spawnPrefabs);
            nm.spawnPrefabs.AddRange(diffGo);
        }
#endif

        private void Start()
        {
            if (Application.isPlaying)
            {
                if (SyncNet.isServer)
                {
                    StartCoroutine(DelaySpawn());
                }
            }
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
                SyncNet.Spawn(go);
            });
        }
    }
}