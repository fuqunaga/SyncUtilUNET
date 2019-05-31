using Mirror;
using System.Collections;
using UnityEngine;

namespace SyncUtil
{
    /// <summary>
    /// DummyPlayer for Mirror.
    /// Mirror spanwns scene object at NetworkServer.AddPlayerForConnection().
    /// </summary>
    [RequireComponent(typeof(NetworkIdentity))]
    public class DummyPlayer : MonoBehaviour
    {
        void Awake()
        {
            StartCoroutine(DestoryAfter());
        }

        IEnumerator DestoryAfter()
        {
            yield return null;
            Destroy(gameObject);
        }
    }
}