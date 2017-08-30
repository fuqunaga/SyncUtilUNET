using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace SyncUtil
{
    [ExecuteInEditMode]
    public class OnlineOfflineSceneLoadHelper : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool _autoUnloadOnline = true;
        public bool _autoUnloadOffline = true;
        public bool _autoLoadOnline = true;
        public bool _autoLoadOffline = true;

        static OnlineOfflineSceneLoadHelper _instance;
        static OnlineOfflineSceneLoadHelper Instance { get { return (_instance != null) ? _instance : (_instance = FindObjectOfType<OnlineOfflineSceneLoadHelper>()); } }

        [InitializeOnLoadMethod]
        public static void Init()
        {
            EditorApplication.playmodeStateChanged += () =>
            {
                var startPlay = !EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode;
                if (startPlay)
                {
                    Instance.UnloadScenes();
                }
            };
        }

        void UnloadScenes()
        {
            var nm = FindObjectOfType<NetworkManager>(); // singleton maybe not ready.
            Assert.IsNotNull(nm);

            Enumerable.Range(0, SceneManager.sceneCount)
            .Select(i => SceneManager.GetSceneAt(i))
            .Where(s => (_autoUnloadOnline && (s.name == nm.onlineScene)) || (_autoUnloadOffline && (s.name == nm.offlineScene)))
            .Where(s => s.isLoaded)
            .ToList()
            .ForEach(scene =>
            {
                if ( scene.isDirty )
                {
                    EditorSceneManager.SaveScene(scene);
                }
                SceneManager.UnloadSceneAsync(scene);
            });
        }

        void Start()
        {
            if (!Application.isPlaying)
            {
                var sceneNameToPath = EditorBuildSettings.scenes.ToDictionary(s => Path.GetFileNameWithoutExtension(s.path), s => s.path);

                var nm = FindObjectOfType<NetworkManager>();
                Assert.IsNotNull(nm);


                new[] {
                _autoLoadOnline ? nm.onlineScene : "",
                _autoLoadOffline ? nm.offlineScene : ""
            }
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