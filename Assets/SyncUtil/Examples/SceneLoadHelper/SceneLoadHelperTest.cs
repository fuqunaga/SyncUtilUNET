using UnityEngine;

namespace SyncUtil.Example
{
    public class SceneLoadHelperTest : MonoBehaviour
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