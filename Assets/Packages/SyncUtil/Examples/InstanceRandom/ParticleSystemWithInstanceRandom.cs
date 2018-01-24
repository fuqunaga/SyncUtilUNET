using UnityEngine;


namespace SyncUtil.Example
{
    [RequireComponent(typeof(ParticleSystem), typeof(InstanceRandom))]
    public class ParticleSystemWithInstanceRandom : MonoBehaviour
    {
        private void Start()
        {
            var ps = GetComponent<ParticleSystem>();
            ps.Stop();
            ps.Clear();

            var instanceRandom = GetComponent<InstanceRandom>();
            ps.randomSeed = (uint)instanceRandom.rand.RandInt();

            ps.Play();
        }
    }
}