using UnityEngine;

namespace LocalClustering
{
    public class LatencyCheckerSample : MonoBehaviour
    {
        NetworkManagerController _networkManagerController;
        LatencyCheckerLine _laytencyCheckerLine;

        void Start()
        {
            _networkManagerController = FindObjectOfType<NetworkManagerController>();
            _laytencyCheckerLine = FindObjectOfType<LatencyCheckerLine>();
            _laytencyCheckerLine.Datas.ForEach(data => data.enable = true);

        }

        private void OnGUI()
        {
            _networkManagerController.DebugMenu();
            _laytencyCheckerLine.DebugMenu();
        }
    }
}