using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SyncUtil.Example
{
    public class PreRenderingDebugMenu : MonoBehaviour
    {
        PreRendering preRendering;
        LatencyCheckerLine laytencyCheckerLine;


        private void OnEnable()
        {
            DebugMenuForExample.Instance.onGUI += DebugMenu;
        }

        private void OnDisable()
        {
            DebugMenuForExample.Instance.onGUI -= DebugMenu;
        }

        void Start()
        {
            preRendering = FindObjectOfType<PreRendering>();

            laytencyCheckerLine = FindObjectOfType<LatencyCheckerLine>();
            StartCoroutine(SetLaytencyCheckerLineEnable());
        }


        IEnumerator SetLaytencyCheckerLineEnable()
        {
            yield return new WaitForEndOfFrame();
            laytencyCheckerLine.Datas.ForEach(data => data.enable = true);

        }

        // Update is called once per frame
        void DebugMenu()
        {
            PrerenderingDebugMenu();
            laytencyCheckerLine.DebugMenu();
        }

        void PrerenderingDebugMenu()
        {
            preRendering.enabled = GUILayout.Toggle(preRendering.enabled, "PreRendering");

            var enable = preRendering.enabled;
            if (enable)
            {
                GUIUtil.Indent(() =>
                {
                    var data = preRendering.data;
                    data.passthrough = GUILayout.Toggle(data.passthrough, "PassThrough");
                    data.delay = GUIUtil.Slider(data.delay, "Delay");
                });
            }
        }
    }
}