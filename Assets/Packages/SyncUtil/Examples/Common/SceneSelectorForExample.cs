using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;

namespace SyncUtil
{
    [ExecuteInEditMode]
    public class SceneSelectorForExample : MonoBehaviour
    {
#if UNITY_EDITOR
        public List<SceneAsset> _onlineScenes = new List<SceneAsset>();

        private void Update()
        {
            _onlineSceneNames = _onlineScenes.Where(s => s != null).Select(s => s.name).ToArray(); ;
        }
#endif

        public int _idx;

        [HideInInspector]
        public string[] _onlineSceneNames;

        private void Start()
        {
            UpdateOnlineScene();
        }

        private void OnValidate()
        {
            UpdateOnlineScene();
        }

        public void DebugMenu()
        {
            using (var h = new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Scene: ");

                var newIdx = GUILayout.SelectionGrid(_idx, _onlineSceneNames, 1);
                if (newIdx != _idx)
                {
                    _idx = newIdx;
                    UpdateOnlineScene();
                }
            }
        }

        void UpdateOnlineScene()
        {
            var nm = FindObjectOfType<NetworkManager>();
            nm.onlineScene = _onlineSceneNames[_idx];
        }
    }
}