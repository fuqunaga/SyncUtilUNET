using UnityEngine;
using System.Linq;

namespace SyncUtil
{
    /// <summary>
    /// Client
    ///  childrens will be deactivated
    /// </summary>
    public class ServerOrStandAlone : MonoBehaviour
    {
        public void Awake()
        {
            var active = SyncNet.isServerOrStandAlone;

            Enumerable.Range(0, transform.childCount)
                .Select(idx => transform.GetChild(idx)).ToList()
                .ForEach(child => child.gameObject.SetActive(active));
        }
    }
}