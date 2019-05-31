using System.Linq;
using UnityEngine;

namespace SyncUtil.Example
{

    public class SyncBehaviourEnableGameObjectActiveExample : MonoBehaviour
    {
        public Behaviour behaviour;
        public GameObject go;

        private void OnEnable()
        {
            DebugMenuForExample.Instance.onGUI += DebugMenu;
        }

        private void OnDisable()
        {
            var menu = DebugMenuForExample.Instance;
            if (menu != null)
            {
                menu.onGUI -= DebugMenu;
            }
        }

        public void DebugMenu()
        {
            GUILayout.Label("Sync BehaviourEnable,GameObjectActive");
            GUIUtil.Indent(() =>
            {
                behaviour.enabled = GUILayout.Toggle(behaviour.enabled, $"Behaviour {behaviour.GetType().ToString().Split('.').Last()}({behaviour.name})");
                go.SetActive(GUILayout.Toggle(go.activeSelf, $"GameObject {go.name}"));
            });
        }
    }
}
