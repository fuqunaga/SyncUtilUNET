using UnityEngine;

namespace SyncUtil
{
    /// <summary>
    /// UnityEngine.Random like random for instance
    /// </summary>
    public class CustomRandom
    {

        System.Func<float> _randFunc;

        public CustomRandom(int seed)
        {
            var rand = new System.Random(seed); // warn: not include 1.0. UnityEngine.Random.value includes 1.0
            _randFunc = () => (float)rand.NextDouble();
        }

        public CustomRandom(System.Func<float> randFunc) { _randFunc = randFunc; }


        #region UnityEndgine.Random like

        public Quaternion rotation => Quaternion.Euler(value*360f, value*360f, value*360f);

        public Vector3 onUnitSphere
        {
            get
            {
                Vector3 ret;
                do
                {
                    ret = (new Vector3(value, value, value) - Vector3.one * 0.5f).normalized;
                } while (ret.sqrMagnitude == 0f);
                return ret;
            }
        }

        public Vector2 insideUnitCircle
        {
            get
            {
                Vector2 ret;
                do
                {
                    ret = (new Vector2(value, value) - Vector2.one * 0.5f) * 2f;
                } while (ret.sqrMagnitude > 1f);

                return ret;
            }
        }

        public Vector3 insideUnitSphere
        {
            get
            {
                Vector3 ret;
                do
                {
                    ret = (new Vector3(value, value, value) - Vector3.one * 0.5f) * 2f;
                } while (ret.sqrMagnitude > 1f);

                return ret;
            }
        }

        public float value { get { return _randFunc(); } }

        public float Range(float min, float max) { return Mathf.Lerp(min, max, value); }

        public int Range(int min, int max) { return Mathf.FloorToInt((max - min) * value * (1f - float.Epsilon)) + min; }

        #endregion


        #region Extra Methods
        public int RandInt()
        {
            return Mathf.FloorToInt(value * int.MaxValue);
        }


        #endregion
    }
}