using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable CS0618

namespace SyncUtil.Example
{
    public class ClientInvisibilityMenu : MonoBehaviour
    {
        public List<NetworkIdentity> prefabs;
        public Rect screenRect = new Rect(Vector2.up * 500f, Vector2.one * 500f);

        void OnGUI()
        {
            if (SyncNet.isServerOrStandAlone)
            {
                using var area = new GUILayout.AreaScope(screenRect, GetType().ToString(), "box");

                GUILayout.Space(32f);

                var i = 0;
                foreach (var prefab in prefabs)
                {
                    if (GUILayout.Button($"Spawn [{prefab.name}]"))
                    {
                        var go = Instantiate(prefab).gameObject;
                        go.transform.position = Vector3.up * i;

                        SyncNet.Spawn(go);
                    }

                    i++;
                }
            }
        }
    }
}