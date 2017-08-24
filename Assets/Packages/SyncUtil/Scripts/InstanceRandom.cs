using UnityEngine;
using UnityEngine.Networking;

namespace SyncUtil
{
    public class InstanceRandom : NetworkBehaviour
    {
        [SyncVar]
        protected int seed;

        public void Awake()
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }

        CustomRandom _rand;
        public CustomRandom rand { get { return (_rand ?? (_rand = new CustomRandom(seed))); } }
    }
}
