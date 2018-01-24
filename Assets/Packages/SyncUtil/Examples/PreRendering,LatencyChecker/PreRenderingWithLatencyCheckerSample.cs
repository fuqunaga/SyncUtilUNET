using SyncUtil;
using UnityEngine;


namespace SyncUtil.Example
{
    public class PreRenderingWithLatencyCheckerSample : PreRenderingWithLatencyChecker
    {
        public bool isEnable = true;
        public bool autoDelay = true;
        public float autoDelayOffset;

        public override bool _enable { get { return isEnable; } }
        public override bool _autoDelay { get { return autoDelay; } }
        public override float _autoDelayOffset { get { return autoDelayOffset; } }


        public void DebugMenu()
        {
            isEnable = GUILayout.Toggle(isEnable, "PreRendering");

            var enable = _preRendering.enabled;
            if (enable)
            {
                GUIUtil.Indent(() =>
                {
                    var data = _preRendering.data;
                    data.passthrough = GUILayout.Toggle(data.passthrough, "PassThrough");

                    GUI.enabled = !autoDelay;
                    data.delay = GUIUtil.Slider(data.delay, "Delay");
                    GUI.enabled = true;

                    autoDelay = GUILayout.Toggle(autoDelay, "AutoDelay");
                    if (autoDelay)
                    {
                        autoDelayOffset = GUIUtil.Slider(autoDelayOffset, -1f, 1f, "AutoDelayOffset");
                    }
                });
            }
        }
    }
}