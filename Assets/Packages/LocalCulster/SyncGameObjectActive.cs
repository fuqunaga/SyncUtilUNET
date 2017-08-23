using System;
using UnityEngine;

namespace LocalClustering
{
    public class SyncGameObjectActive : SyncObjectBool<GameObject>
    {
        protected override bool Get(GameObject target)
        {
            return target.activeSelf;
        }

        protected override void Set(GameObject target, bool value)
        {
            target.SetActive(value);
        }
    }
}