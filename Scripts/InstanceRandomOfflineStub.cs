using UnityEngine;

namespace SyncUtil
{
    public class InstanceRandomOfflineStub : MonoBehaviour, IInstanceRandom
    {
        protected int seed;

        public void Awake()
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }

        CustomRandom _rand;
        public CustomRandom rand { get { return (_rand ?? (_rand = new CustomRandom(seed))); } }
    }
}
