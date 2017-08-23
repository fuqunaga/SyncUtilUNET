using UnityEngine;

namespace LocalClustering
{
    public class LantencyCheckerSample : MonoBehaviour
    {
        LatencyCheckerLine _laytencyCheckerLine;

        void Start()
        {
            _laytencyCheckerLine = FindObjectOfType<LatencyCheckerLine>();
            _laytencyCheckerLine.Datas.ForEach(data => data.enable = true);

        }

        private void OnGUI()
        {
            _laytencyCheckerLine.DebugMenu();
        }
    }
}