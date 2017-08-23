using UnityEngine;
using LocalClustering;

public class NetworkTimeForShader : MonoBehaviour
{
    public class ShaderParam
    {
        public const string Time = "g_Time";
    }

    public void Update()
    {
        var time = (float)LocalCluster.networkTime;
        Shader.SetGlobalVector(ShaderParam.Time, new Vector4(time / 20f, time, time * 2f, time * 3f));
    }
}
