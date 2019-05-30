using Mirror;
using SyncUtil;
using System.Linq;
using UnityEngine;

namespace SyncUtil
{
    [RequireComponent(typeof(PreRendering))]
    public class PreRenderingWithLatencyChecker : MonoBehaviour
    {
        public virtual bool enable { get; } = true;
        public virtual float delayOffset { get; }

        protected PreRendering preRendering;

        public void Start()
        {
            preRendering = GetComponent<PreRendering>();
            UpdateParams();
        }

        public void Update()
        {
            UpdateParams();
        }

        void UpdateParams()
        {
            preRendering.enabled = enable && SyncNet.isServerOrStandAlone;

            var data = preRendering.data;
            if (delayOffset > 0f)
            {
                data.delay = delayOffset;
            }
        }
    }
}