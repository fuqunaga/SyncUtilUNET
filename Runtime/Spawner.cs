using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

namespace SyncUtil
{
    [ExecuteAlways]
    public class Spawner : MonoBehaviour
    {
        public List<NetworkIdentity> prefabs = new List<NetworkIdentity>();

#if UNITY_EDITOR
        // Auto Regist to SpawnPrefabs on Editor
        NetworkManager networkManager;
        private void Update()
        {
            var nm = networkManager ?? (networkManager = FindObjectOfType<NetworkManager>());
            var diffGo = prefabs.Where(ni => ni!=null).Select(ni => ni.gameObject).Except(nm.spawnPrefabs);
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
            yield return new WaitUntil(() => NetworkServer.active);

            prefabs
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