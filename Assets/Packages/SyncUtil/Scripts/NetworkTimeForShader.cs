using UnityEngine;
using SyncUtil;

public class NetworkTimeForShader : MonoBehaviour
{
    public class ShaderParam
    {
        public const string Time = "g_Time";
    }

    public void Update()
    {
        var time = (float)SyncNet.networkTime;
        Shader.SetGlobalVector(ShaderParam.Time, new Vector4(time / 20f, time, time * 2f, time * 3f));
    }
}
