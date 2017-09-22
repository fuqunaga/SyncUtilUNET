using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SyncUtil.Example
{

    public class GPUFluidMouse : MonoBehaviour
    {

        public float _radius = 0.1f;
        public Color _color = Color.red;
        public bool _isHueAnim = true;

        public Vector2 _pos;
        public Vector2 _velocity;
        public Vector2? _lastMousePos;

        public bool isDataValid => _lastMousePos.HasValue;

        void Update()
        {
            if (_isHueAnim)
            {
                float h, s, v;
                Color.RGBToHSV(_color, out h, out s, out v);
                h += Time.deltaTime * 0.1f;
                _color = Color.HSVToRGB(h, s, v);
            }

            if (!Input.GetMouseButton(0))
            {
                _lastMousePos = null;
            }
            else
            {
                Vector2 pos = Input.mousePosition;
                if (_lastMousePos.HasValue)
                {
                    _pos = Camera.main.ScreenToViewportPoint(pos);
                    _velocity = pos - _lastMousePos.Value;
                }

                _lastMousePos = pos;
            }
        }

    }
}