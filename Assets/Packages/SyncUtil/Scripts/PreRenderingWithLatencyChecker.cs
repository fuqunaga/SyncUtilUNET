using SyncUtil;
using System.Linq;
using UnityEngine;

namespace SyncUtil
{
    [RequireComponent(typeof(PreRendering))]
    public class PreRenderingWithLatencyChecker : MonoBehaviour
    {
        public virtual bool _enable { get; } = true;
        public virtual bool _autoDelay { get; } = true;
        public virtual float _autoDelayOffset { get; }

        protected PreRendering _preRendering;

        public void Start()
        {
            _preRendering = GetComponent<PreRendering>();
            UpdateParams();
        }

        public void Update()
        {
            UpdateParams();
        }

        void UpdateParams()
        {
            _preRendering.enabled = _enable && SyncNet.isServerOrStandAlone;

            var data = _preRendering.data;
            if (_autoDelay)
            {
                var datas = LatencyChecker.Instance._conectionLatencyTable.Values;
                var latency = datas.Any() ? datas.Select(d => d.average).Average() : 0f; // 複数のClientがある場合は考えてない。平均の平均じゃまずい
                data.delay = latency + _autoDelayOffset;
            }
        }

        /*
        public void DebugMenu()
        {
            _enable.OnGUI("PreRendering" + (SyncNet.isSlaver ? "(Always Off on Client)" : ""));

            var enable = _preRendering.enabled;
            if (enable)
            {
                GUIUtil.Indent(() =>
                {
                    var data = _preRendering.data;
                    data.passthrough = GUILayout.Toggle(data.passthrough, "PassThrough");

                    GUI.enabled = _autoDelay;
                    data.delay = GUIUtil.Slider(data.delay, "Delay");
                    GUI.enabled = true;

                    _autoDelay.OnGUI("AutoDelay");
                    if (_autoDelay)
                    {
                        _autoDelayOffset.OnGUISlider(-1f, 1f, "AutoDelayOffset");
                    }
                });
            }
        }
        */
    }
}