using UnityEngine;

namespace SyncUtil
{

    /// <summary>
    /// Unity's _Time style shader property
    /// </summary>
    public class NetworkTimeForShader : MonoBehaviour
    {
        public string _propertyName = "g_Time";

        public void Update()
        {
           
            Shader.SetGlobalVector(_propertyName, GetVector4Time() );
        }

		public static Vector4 GetVector4Time()
		{
			var time = (float)SyncNet.networkTime;
			return new Vector4(time / 20f, time, time * 2f, time * 3f);
		}
    }

}