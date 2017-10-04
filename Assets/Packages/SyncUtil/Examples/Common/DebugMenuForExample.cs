using System;
using UnityEngine;

namespace SyncUtil
{
    public class DebugMenuForExample : MonoBehaviour
    {
        #region singleton
        static DebugMenuForExample _instance;
        public static DebugMenuForExample Instance => (_instance != null ? _instance : (_instance = FindObjectOfType<DebugMenuForExample>()));
        #endregion

        NetworkManagerController _networkManagerController;

        public Action onGUI;


        void Start()
        {
            _networkManagerController = FindObjectOfType<NetworkManagerController>();
        }

        private void OnGUI()
        {
            if (SyncNet.isActive)
            {
                _networkManagerController.DebugMenu();

                onGUI?.Invoke();
            }
        }
    }
}