using UnityEngine;
using UnityEngine.Networking;

namespace SyncUtil.Example
{
    public class LockStepExampleBase : MonoBehaviour
    {
        protected bool _stepEnable = true;

        protected virtual void Start()
        {
            var debugMenu = FindObjectOfType<DebugMenuForExample>();
            if (debugMenu != null)
            {
                debugMenu.onGUI += DebugMenu;
            }
        }

        protected virtual  void OnDestroy()
        {
            var debugMenu = FindObjectOfType<DebugMenuForExample>();
            if (debugMenu != null)
            {
                debugMenu.onGUI -= DebugMenu;
            }
        }

        protected virtual void DebugMenu()
        {
            if (SyncNet.isClient)
            {
                using (var h = new GUILayout.HorizontalScope())
                {
                    _stepEnable = GUILayout.Toggle(_stepEnable, "StepEnable");
                }
            }
        }

    }
}
