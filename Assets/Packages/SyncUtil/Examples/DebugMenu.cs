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
            _laytencyCheckerLine.Datas.ForEach(data => data.enable = true);

        }

        private void OnGUI()
        {
            _networkManagerController.DebugMenu();
            _preRendering.DebugMenu();
            _laytencyCheckerLine.DebugMenu();
        }
    }
}