using UnityEngine;

namespace SyncUtil
{
    public class DebugMenu : MonoBehaviour
    {
        NetworkManagerController _networkManagerController;
        PreRenderingWithLatencyCheckerSample _preRendering;
        LatencyCheckerLine _laytencyCheckerLine;

        void Start()
        {
            _networkManagerController = FindObjectOfType<NetworkManagerController>();

            _preRendering = FindObjectOfType<PreRenderingWithLatencyCheckerSample>();

            _laytencyCheckerLine = FindObjectOfType<LatencyCheckerLine>();
            if ( _laytencyCheckerLine) _laytencyCheckerLine.Datas.ForEach(data => data.enable = true);

        }

        private void OnGUI()
        {
            _networkManagerController.DebugMenu();
            if ( _preRendering != null ) _preRendering.DebugMenu();
            if (_laytencyCheckerLine != null ) _laytencyCheckerLine.DebugMenu();
        }
    }
}