using UnityEngine;
using SyncUtil;

public class NetworkTimeForShader : MonoBehaviour
{
    public string _propertyName = "g_Time";

    public void Update()
    {
        var time = (float)SyncNet.networkTime;
        Shader.SetGlobalVector(_propertyName, new Vector4(time / 20f, time, time * 2f, time * 3f));
    }
}
