using UnityEngine;

namespace SyncUtil
{
    /// <summary>
    /// this logs will not be called when it is in online/offline scene
    /// </summary>
    public class UnloadOnlineOfflineScenes : MonoBehaviour
    {
        string sceneName { get { return gameObject.scene.name; } }

        private void Awake()
        {
            Debug.Log("Awake: " + sceneName);
        }

        private void OnEnable()
        {
            Debug.Log("OnEnable" + sceneName);
        }

        private void Start()
        {
            Debug.Log("Start" + sceneName);
        }
    }

}