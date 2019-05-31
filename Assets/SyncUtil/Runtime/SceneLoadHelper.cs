using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Mirror;
using System.Collections.Generic;

namespace SyncUtil
{
    /// <summary>
    /// Auto load scenes when editor
    /// Auto unload scene before editor play
    /// </summary>
    [ExecuteAlways]
    public class SceneLoadHelper : MonoBehaviour
    {
#if UNITY_EDITOR


        [Scene]
        public List<string> scenes = new List<string>();

        static SceneLoadHelper instance;
        static SceneLoadHelper Instance { get { return (instance != null) ? instance : (instance = FindObjectOfType<SceneLoadHelper>()); } }

        [InitializeOnLoadMethod]
        public static void Init()
        {
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
        }

        static void PlayModeStateChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.ExitingEditMode && (Instance != null))
            {
                Instance.UnloadScenes();
            }
        }

        void UnloadScenes()
        {
            if (isActiveAndEnabled)
            {
                scenes
                    .Select(name => SceneManager.GetSceneByName(name))
                    .Where(s => s.isLoaded)
                    .ToList()
                    .ForEach(scene =>
                    {
                        if (scene.isDirty)
                        {
                            EditorSceneManager.SaveScene(scene);
                        }
                        SceneManager.UnloadSceneAsync(scene);
                    });
            }
        }

        void Start()
        {
            if (!Application.isPlaying)
            {
                var sceneNameToPath = EditorBuildSettings.scenes.ToDictionary(s => Path.GetFileNameWithoutExtension(s.path), s => s.path);

                scenes.ForEach(name =>
                {
                    EditorSceneManager.OpenScene(sceneNameToPath[name], OpenSceneMode.Additive);
                });
            }
        }
#endif
    }

}