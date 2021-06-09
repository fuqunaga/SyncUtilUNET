using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 0618

namespace SyncUtil
{
    [ExecuteAlways]
    public class Spawner : MonoBehaviour
    {
        public List<NetworkIdentity> _prefabs = new List<NetworkIdentity>();

#if UNITY_EDITOR
        // Auto Regist to SpawnPrefabs on Editor
        NetworkManager _networkManager;
        private void Update()
        {
            if (!Application.isPlaying)
            {
                var nm = _networkManager ?? (_networkManager = FindObjectOfType<NetworkManager>());
                if (nm != null)
                {
                    var diffGo = _prefabs.Where(ni => ni != null).Select(ni => ni.gameObject).Except(nm.spawnPrefabs);
                    nm.spawnPrefabs.AddRange(diffGo);
                }
            }
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
            yield return new WaitUntil(() => NetworkServer.active);

            foreach(var prefab in _prefabs)
            {
                var go = Instantiate(prefab.gameObject);
                go.transform.SetParent(transform);
                SyncNet.Spawn(go);
            }
        }
    }
}