using UnityEngine;

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

            for (var i = 0; i < transform.childCount; ++i)
            {
                transform.GetChild(i).gameObject.SetActive(active);
            }
        }
    }
}