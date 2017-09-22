using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SyncUtil.Example
{


    public class GPUFuildToCamera : MonoBehaviour
    {

        public enum TexType
        {
            Color,
            Velocity,
            Pressure,
        }

        public TexType _type;
        public Shader _shader;
        Material _material;

        GPUFluid _fluid;

        private void Start()
        {
            _material = new Material(_shader);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_fluid == null)
            {
                _fluid = FindObjectOfType<GPUFluid>();
            }
            else
            {
                var tex = new[] { _fluid._color, _fluid._velocity, _fluid._pressure }[(int)_type];
                Graphics.Blit(tex, destination, _material);
            }
        }

        private void OnDestroy()
        {
            Destroy(_material);
        }
    }

}