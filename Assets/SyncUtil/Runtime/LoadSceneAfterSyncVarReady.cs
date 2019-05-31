using Mirror;
using UnityEngine.SceneManagement;

namespace SyncUtil
{
    /// <summary>
    /// クライアントではNetworkManager.onlineSceneはシーン上のNetworkBehaviourがSpawnされるまえにロードされる
    /// したがってSyncParamManagerなどの値が同期されるまえにonlineSceneが動き出してしまう
    /// SyncVarを使用するNetworkBehaviourのみをonlineSceneに置いておき、
    /// 準備ができてからLoadSceneAfterSyncVarReadyでSync済みのパラメータを使うシーンをロードすると安全
    /// </summary>
    public class LoadSceneAfterSyncVarReady : NetworkBehaviour
    {
        [Scene]
        public string scene;


        public override void OnStartServer()
        {
            SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        }

        public override void OnStartClient()
        {
            if ( isClientOnly )
            {
                SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            }
        }
    }
}