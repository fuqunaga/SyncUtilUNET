using System;
using UnityEngine;

namespace SyncUtil.Example
{
    public class DebugMenuForExample : MonoBehaviour
    {
        #region singleton
        static DebugMenuForExample instance;
        public static DebugMenuForExample Instance => (instance != null ? instance : (instance = FindObjectOfType<DebugMenuForExample>()));
        #endregion

        NetworkManagerController networkManagerController;

        public Action onGUI;


        void Start()
        {
            networkManagerController = FindObjectOfType<NetworkManagerController>();
        }

        private void OnGUI()
        {
            if (SyncNet.isActive)
            {
                networkManagerController.DebugMenu();

                onGUI?.Invoke();
            }
        }
    }
}