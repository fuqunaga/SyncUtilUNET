using UnityEngine;
using UnityEngine.Networking;

namespace SyncUtil.Example
{
    [RequireComponent(typeof(LockStep))]
    public class LockStepDebugMenu : MonoBehaviour
    {
        LockStep _lockStep;
        DebugMenuForExample _debugMenu;

        void Start()
        {
            _lockStep = GetComponent<LockStep>();

            _debugMenu = FindObjectOfType<DebugMenuForExample>();
            _debugMenu.onGUI += _DebugMenu;
        }

        private void OnDestroy()
        {
            _debugMenu.onGUI -= _DebugMenu;
        }

        void _DebugMenu()
        {
            if (SyncNet.isServer)
            {
                DebugMenu(_lockStep);
            }
        }

        public static void DebugMenu(LockStep lockStep)
        {

            var connectionCount = NetworkServer.connections.Count;
            GUILayout.Label("Connection Count: " + connectionCount);

            if (connectionCount >= 2)
            {
                using (var h = new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("CheckConsistency"))
                    {
                        lockStep.StartCheckConsistency();
                    }

                    var consistency = lockStep.GetLastConsistency();
                    GUILayout.Label(consistency.ToString());
                }
            }
        }
    }
}