using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace SyncUtil
{
    [ExecuteInEditMode]
    public class AutoLoadOnlineOfflineScene : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Start()
        {
            if (!Application.isPlaying)
            {
                var sceneNameToPath = EditorBuildSettings.scenes.ToDictionary(s => Path.GetFileNameWithoutExtension(s.path), s => s.path);

                var nm = FindObjectOfType<NetworkManager>();
                Assert.IsNotNull(nm);


                new[] { nm.onlineScene, nm.offlineScene }
                .Where(sceneName => !string.IsNullOrEmpty(sceneName))
                .ToList()
                .ForEach(sceneName =>
                {
                    EditorSceneManager.OpenScene(sceneNameToPath[sceneName], OpenSceneMode.Additive);
                });
            }
        }
#endif
    }

}