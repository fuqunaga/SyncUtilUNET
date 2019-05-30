using UnityEngine;
using Mirror;

namespace SyncUtil.Example
{
    [RequireComponent(typeof(ILockStep))]
    public class LockStepDebugMenu : MonoBehaviour
    {
        ILockStep _lockStep;
        DebugMenuForExample _debugMenu;

        void Start()
        {
            _lockStep = GetComponent<ILockStep>();

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

        public static void DebugMenu(ILockStep lockStep)
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

                    var data = lockStep.GetLastConsistencyData();
                    GUILayout.Label(data.consistency + " step:" + data.stepCount);
                }
            }
        }
    }
}