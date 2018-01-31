using UnityEngine;


namespace SyncUtil.Example
{
    [RequireComponent(typeof(ParticleSystem), typeof(IInstanceRandom))]
    public class ParticleSystemWithInstanceRandom : MonoBehaviour
    {
        private void Start()
        {
            var ps = GetComponent<ParticleSystem>();
            ps.Stop();
            ps.Clear();

            var instanceRandom = GetComponent<IInstanceRandom>();
            ps.randomSeed = (uint)instanceRandom.rand.RandInt();

            ps.Play();
        }
    }
}