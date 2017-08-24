using UnityEngine;
using SyncUtil;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;

public class ServerOrStandAlone : MonoBehaviour
{
    public List<GameObject> _spawnPrefabs;

    public void Awake()
    {
        var active = SyncNet.isServerOrStandAlone;

        Enumerable.Range(0, transform.childCount)
            .Select(idx => transform.GetChild(idx)).ToList()
            .ForEach(child => child.gameObject.SetActive(active));


        if (active)
        {
            _spawnPrefabs.ForEach(p =>
            {
                var go = Instantiate(p);
                go.transform.SetParent(transform);

                NetworkServer.Spawn(go);
            });
        }
    }
}
