using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 0618

namespace SyncUtil
{
    public interface IInstanceRandom
    {
        CustomRandom rand { get; }
    }


    public class InstanceRandom : NetworkBehaviour, IInstanceRandom
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
