namespace LocalClustering
{
    public class NetworkManagerControllerSample : NetworkManagerController
    {
        public string networkAddress = "localhost";
        public int networkPort = 7777;
        public BootType bootType = BootType.Manual;
        public bool autoConnect = true;
        public float autoConnectInterval = 10f;

        public override string _networkAddress { get { return networkAddress; } }
        public override int _networkPort { get { return networkPort; } }
        public override BootType _bootType { get { return bootType; } }
        public override bool _autoConnect { get { return autoConnect; } }
        public override float _autoConnectInterval { get { return autoConnectInterval; } }

        protected override void DebugMenuInternal()
        {
            networkAddress = GUIUtil.Field(networkAddress, "Host Address");
            networkPort = GUIUtil.Field(networkPort, "Host Port");
            bootType = GUIUtil.Field(bootType, "Boot Type");
            autoConnect = GUIUtil.Field(autoConnect, "AutoConnect");
            autoConnectInterval = GUIUtil.Field(autoConnectInterval, "AutoConnectInterval");
        }
    }
}
