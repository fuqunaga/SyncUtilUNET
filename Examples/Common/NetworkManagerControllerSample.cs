using UnityEngine;

namespace SyncUtil.Example
{
    [RequireComponent(typeof(SceneSelectorForExample))]
    public class NetworkManagerControllerSample : NetworkManagerController
    {
        public string networkAddress = "localhost";
        public int networkPort = 7777;
        public BootType bootType = BootType.Manual;
        public bool autoConnect = true;
        public float autoConnectInterval = 10f;

        public override string _networkAddress { get { return networkAddress; } }
        public override BootType _bootType { get { return bootType; } }
        public override bool _autoConnect { get { return autoConnect; } }
        public override float _autoConnectInterval { get { return autoConnectInterval; } }

        SceneSelectorForExample _sceneSelector;

        public override void Start()
        {
            base.Start();
            _sceneSelector = GetComponent<SceneSelectorForExample>();
        }

        protected override void OnGUINetworkSetting()
        {
            _sceneSelector.DebugMenu();

            networkAddress = GUIUtil.Field(networkAddress, "Host Address");
            networkPort = GUIUtil.Field(networkPort, "Host Port");
            
            DebugMenuInternal();
        }

        protected override void DebugMenuInternal()
        {
            autoConnect = GUIUtil.Field(autoConnect, "AutoConnect");
            autoConnectInterval = GUIUtil.Field(autoConnectInterval, "AutoConnectInterval");
        }
    }
}
