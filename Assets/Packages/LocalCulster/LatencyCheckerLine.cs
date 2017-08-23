using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace LocalClustering
{
    /// <summary>
    /// パラメータを直線で表し、複数PCでの差異を視覚的に比べる表示
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class LatencyCheckerLine : MonoBehaviour
    {
        public Color _timeColor = Color.white;
        public Color _networkTimeColor = Color.gray;

        public enum Mode
        {
            Horizontal,
            Vertical
        }

        public class CameraData
        {
            public Camera camera;
            public bool enable;
            public Mode mode;

            protected DebugDraw _debugDraw;

            public string name { get { return camera.name; } }
            public DebugDraw debugDraw { get { return _debugDraw ?? (_debugDraw = (camera.GetComponent<DebugDraw>() ?? camera.gameObject.AddComponent<DebugDraw>())); } }
        }

        List<CameraData> _datas = new List<CameraData>();
        public List<CameraData> Datas { get { return _datas; } }

        float _width = 5f;
        bool _networkTimeEnable = true;
        float _networkTimeStride = 5f;

        bool _timeEnable = true;
        float _timeStride = 5f;

        protected virtual void Start()
        {
            _datas.Add(new CameraData() { camera = GetComponent<Camera>() });
        }

        public void OnPreRender()
        {
            var datas = _datas.Where(data => data.enable).ToList();
            OnPreRenderDatas(datas);
        }

        protected virtual void OnPreRenderDatas(List<CameraData> datas)
        {
            datas.ForEach(data =>
            {
                if (_timeEnable) DrawLine(data, LocalCluster.time, _timeStride, _timeColor);
                if (_networkTimeEnable) DrawLine(data, (float)LocalCluster.networkTime, _networkTimeStride, _networkTimeColor);
            });
        }


        protected void DrawLine(CameraData data, Vector3 val, float stride, params Color[] col)
        {
            for (var i = 0; i < 3; ++i) DrawLine(data, val[i], stride, col[i]);
        }

        protected void DrawLine(CameraData data, float val, float stride, Color col)
        {
            if (stride > 0f)
            {
                var rate = (val % stride) / stride;
                var lb = Vector2.zero;
                var rt = Vector2.one;
                var idx = data.mode == Mode.Horizontal ? 1 : 0;

                lb[idx] = rt[idx] = rate;

                data.debugDraw.LineOn2D(lb, rt, col, _width);
            }
        }


        public virtual void DebugMenu()
        {
            enabled = GUILayout.Toggle(enabled, "LatencyChckerLine");

            if (enabled)
            {
                GUIUtil.Indent(() =>
                {
                    _width = GUIUtil.Slider(_width, 0f, 20f, "width");

                    using (var h = new GUILayout.HorizontalScope())
                    {
                        _timeEnable = GUILayout.Toggle(_timeEnable, "Time");
                        _timeStride = GUIUtil.Slider(_timeStride, 0.1f, 10f, "Stride");
                    }

                    using (var h = new GUILayout.HorizontalScope())
                    {
                        _networkTimeEnable = GUILayout.Toggle(_networkTimeEnable, "NetworkTime");
                        _networkTimeStride = GUIUtil.Slider(_networkTimeStride, 0.1f, 10f, "Stride");
                    }

                    DebugMenuInternal();

                    _datas.ForEach(data =>
                    {
                        using (var h = new GUILayout.HorizontalScope())
                        {
                            data.enable = GUILayout.Toggle(data.enable, data.name);
                            data.mode = GUIUtil.Field(data.mode);
                        }
                    });

                });
            }
        }

        protected virtual void DebugMenuInternal() { }
    }
}