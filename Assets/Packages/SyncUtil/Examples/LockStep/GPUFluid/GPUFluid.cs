using System.Linq;
using UnityEngine;

    namespace SyncUtil.Example
    {

    public class GPUFluid : MonoBehaviour
    {

        public static class CSPARAM
        {
            public const string KERNEL_ADVECT_FLOAT2 = "AdvectFloat2";
            public const string KERNEL_ADVECT_FLOAT4 = "AdvectFloat4";
            public const string KERNEL_DIFFUSE_FLOAT2 = "DiffuseFloat2";
            public const string KERNEL_DIFFUSE_FLOAT4 = "DiffuseFloat4";
            public const string KERNEL_ADD_FORCE = "AddForce";
            public const string KERNEL_COMPUTE_DIVERGENCE = "ComputeDivergence";
            public const string KERNEL_SOLVE_PRESSURE = "SolvePressure";
            public const string KERNEL_PRESSURE_GRADIENT_SUBTRACT = "PressureGradientSubtract";

            public const string DELTA_TIME = "_DeltaTime";
            public const string VELOCITY_TO_UV = "_VelocityToUV";
            public const string VISCOCITY = "_Viscocity";
            public const string ADDFORCE_VELOCITY = "_AddForceVelocity";
            public const string ADD_POS = "_AddPos";
            public const string ADD_RADIUS = "_AddRadius";
            public const string ADD_COLOR = "_AddColor";

            // Textureas
            public const string ADVECT_FLOAT2_WRITE = "_AdvectFloat2Write";
            public const string ADVECT_FLOAT2_READ = "_AdvectFloat2Read";
            public const string ADVECT_FLOAT4_WRITE = "_AdvectFloat4Write";
            public const string ADVECT_FLOAT4_READ = "_AdvectFloat4Read";
            public const string DIFFUSE_FLOAT2_WRITE = "_DiffuseFloat2Write";
            public const string DIFFUSE_FLOAT2_READ = "_DiffuseFloat2Read";
            public const string DIFFUSE_FLOAT4_WRITE = "_DiffuseFloat4Write";
            public const string DIFFUSE_FLOAT4_READ = "_DiffuseFloat4Read";

            public const string VELOCITY = "_Velocity";
            public const string VELOCITY_READ = "_VelocityRead";
            public const string DIVERGENCE = "_Divergence";
            public const string PRESSURE = "_Pressure";
            public const string PRESSURE_READ = "_PressureRead";
            public const string COLOR = "_Color";

        }
        public ComputeShader _cs;


        public RenderTexture _velocity;
        public RenderTexture _pressure;
        public RenderTexture _color;

        public float _velocityToUV = 10;
        public float _viscocity = 10;
        public int _diffuseIteration = 10;
        public int _pressureSolverIteration = 10;

        public int _width = 1024;
        public int _height = 1024;


        private void Start()
        {
            _velocity = new RenderTexture(_width, _height, 0, RenderTextureFormat.RGFloat);
            _pressure = new RenderTexture(_width, _height, 0, RenderTextureFormat.RFloat);
            _color = new RenderTexture(_width, _height, 0, RenderTextureFormat.ARGB32);
            _velocity.wrapMode = TextureWrapMode.Clamp;
            _pressure.wrapMode = TextureWrapMode.Clamp;
            _velocity.enableRandomWrite = true;
            _pressure.enableRandomWrite = true;
            _color.enableRandomWrite = true;
            _velocity.Create();
            _pressure.Create();
            _color.Create();

            var tex = new Texture2D(_width, _height);
            tex.SetPixels(0, 0, _width / 2, _height, Enumerable.Range(0, _width * _height / 2).Select(_ => Color.red).ToArray());
            tex.Apply();
            Graphics.Blit(tex, _color);
            Destroy(tex);
        }

        private void OnDestroy()
        {
            _velocity.Release();
            _pressure.Release();
            _color.Release();
        }



        public class StepData
        {
            public float deltaTime;

            public bool isAddDataValid;
            public Vector2 addPos;
            public float addRadius;
            public Vector2 addVelocity;
            public Color addColor;
        }

        public void Step(StepData data)
        {
            _cs.SetFloat(CSPARAM.DELTA_TIME, data.deltaTime);
            _cs.SetFloat(CSPARAM.VELOCITY_TO_UV, _velocityToUV);

            AddForce(data);

            UpdateFluid();
            UpdateColor();
        }


        void AddForce(StepData data)
        {
            if (data.isAddDataValid)
            {
                var kernel = _cs.FindKernel(CSPARAM.KERNEL_ADD_FORCE);
                _cs.SetVector(CSPARAM.ADD_POS, data.addPos);
                _cs.SetVector(CSPARAM.ADDFORCE_VELOCITY, data.addVelocity);
                _cs.SetFloat(CSPARAM.ADD_RADIUS, data.addRadius);
                _cs.SetVector(CSPARAM.ADD_COLOR, data.addColor);

                _cs.SetTexture(kernel, CSPARAM.VELOCITY, _velocity);
                _cs.SetTexture(kernel, CSPARAM.COLOR, _color);

                _cs.DispatchThreads(kernel, _velocity.width, _velocity.height, 1);
            }
        }




        void UpdateFluid()
        {
            var tmpVelocity = RenderTexture.GetTemporary(_velocity.descriptor);
            tmpVelocity.Create();

            AdvectVelocity(tmpVelocity);
            DiffuseVelocity(tmpVelocity);

            RenderTexture.ReleaseTemporary(tmpVelocity);


            var tmpDivergence = RenderTexture.GetTemporary(_pressure.descriptor);
            tmpDivergence.Create();

            ComputeDivergence(tmpDivergence);
            SolvePressure(tmpDivergence);

            RenderTexture.ReleaseTemporary(tmpDivergence);

            PressureGradientSubtract();
        }

        void UpdateColor()
        {
            AdvectColor();
            DiffuseColor();
        }

        public struct AdvectData
        {
            public string kernelName;
            public string readName;
            public string writeName;
            public Texture readTex;
            public Texture writeTex;
        }

        void AdvectVelocity(RenderTexture tmpVelocity)
        {
            Advect(new AdvectData()
            {
                kernelName = CSPARAM.KERNEL_ADVECT_FLOAT2,
                readName = CSPARAM.ADVECT_FLOAT2_READ,
                writeName = CSPARAM.ADVECT_FLOAT2_WRITE,
                readTex = _velocity,
                writeTex = tmpVelocity
            });
        }

        void AdvectColor()
        {
            var tmp = RenderTexture.GetTemporary(_color.descriptor);
            tmp.Create();
            Graphics.Blit(_color, tmp);

            Advect(new AdvectData()
            {
                kernelName = CSPARAM.KERNEL_ADVECT_FLOAT4,
                readName = CSPARAM.ADVECT_FLOAT4_READ,
                writeName = CSPARAM.ADVECT_FLOAT4_WRITE,
                readTex = tmp,
                writeTex = _color
            });

            RenderTexture.ReleaseTemporary(tmp);
        }

        void Advect(AdvectData data)
        {
            var kernel = _cs.FindKernel(data.kernelName);
            _cs.SetTexture(kernel, CSPARAM.VELOCITY_READ, _velocity);
            _cs.SetTexture(kernel, data.readName, data.readTex);
            _cs.SetTexture(kernel, data.writeName, data.writeTex);

            _cs.DispatchThreads(kernel, data.writeTex.width, data.writeTex.height, 1);
        }



        public struct DiffuseData
        {
            public string kernelName;
            public string readName;
            public string writeName;
            public string viscocityName;
            public RenderTexture readTex;
            public RenderTexture writeTex;
            public float viscocity;
            public int iteration;
        }

        void DiffuseVelocity(RenderTexture tmpVelocity)
        {
            Diffuse(new DiffuseData()
            {
                kernelName = CSPARAM.KERNEL_DIFFUSE_FLOAT2,
                readName = CSPARAM.DIFFUSE_FLOAT2_READ,
                writeName = CSPARAM.DIFFUSE_FLOAT2_WRITE,
                readTex = tmpVelocity,
                writeTex = _velocity,
                viscocity = _viscocity,
                iteration = _diffuseIteration
            });
        }

        public float _colorViscocity = 1;
        public int _colorIteration = 10;

        void DiffuseColor()
        {
            var tmp = RenderTexture.GetTemporary(_color.descriptor);
            tmp.Create();
            Graphics.Blit(_color, tmp);

            Diffuse(new DiffuseData()
            {
                kernelName = CSPARAM.KERNEL_DIFFUSE_FLOAT4,
                readName = CSPARAM.DIFFUSE_FLOAT4_READ,
                writeName = CSPARAM.DIFFUSE_FLOAT4_WRITE,
                readTex = tmp,
                writeTex = _color,
                viscocity = _colorViscocity,
                iteration = _colorIteration
            });

            RenderTexture.ReleaseTemporary(tmp);
        }

        void Diffuse(DiffuseData data)
        {
            var kernel = _cs.FindKernel(data.kernelName);
            _cs.SetFloat(CSPARAM.VISCOCITY, data.viscocity);

            // warn: consiser first swap
            var read = data.writeTex;
            var write = data.readTex;

            for (var i = 0; i < data.iteration; ++i)
            {
                var tmp = read;
                read = write;
                write = tmp;

                _cs.SetTexture(kernel, data.readName, read);
                _cs.SetTexture(kernel, data.writeName, write);

                _cs.DispatchThreads(kernel, write.width, write.height, 1);
            }

            if (data.iteration % 2 == 0)
            {
                Graphics.Blit(read, write);
            }
        }


        void ComputeDivergence(RenderTexture tmpDivergence)
        {
            var kernel = _cs.FindKernel(CSPARAM.KERNEL_COMPUTE_DIVERGENCE);
            _cs.SetTexture(kernel, CSPARAM.VELOCITY_READ, _velocity);
            _cs.SetTexture(kernel, CSPARAM.DIVERGENCE, tmpDivergence);

            _cs.DispatchThreads(kernel, tmpDivergence.width, tmpDivergence.height, 1);
        }

        void SolvePressure(RenderTexture tmpDivergence)
        {
            var kernel = _cs.FindKernel(CSPARAM.KERNEL_SOLVE_PRESSURE);
            _cs.SetTexture(kernel, CSPARAM.DIVERGENCE, tmpDivergence);

            var pressureRead = RenderTexture.GetTemporary(_pressure.descriptor);
            var pressureWrite = _pressure;

            for (var i = 0; i < _pressureSolverIteration; ++i)
            {
                var tmp = pressureRead;
                pressureRead = pressureWrite;
                pressureWrite = tmp;

                _cs.SetTexture(kernel, CSPARAM.PRESSURE_READ, pressureRead);
                _cs.SetTexture(kernel, CSPARAM.PRESSURE, pressureWrite);
                _cs.DispatchThreads(kernel, pressureWrite.width, pressureWrite.height, 1);
            }

            if (_pressureSolverIteration % 2 == 1)
            {
                Graphics.Blit(pressureWrite, _pressure);
            }
        }

        void PressureGradientSubtract()
        {
            var kernel = _cs.FindKernel(CSPARAM.KERNEL_COMPUTE_DIVERGENCE);
            _cs.SetTexture(kernel, CSPARAM.PRESSURE_READ, _pressure);
            _cs.SetTexture(kernel, CSPARAM.VELOCITY, _velocity);

            _cs.DispatchThreads(kernel, _velocity.width, _velocity.height, 1);
        }


        public void ClearRenderTexture(RenderTexture target, Color bg)
        {
            var active = RenderTexture.active;
            RenderTexture.active = target;
            GL.Clear(true, true, bg);
            RenderTexture.active = active;
        }
    }

    public static class ComputeShaderExtenstion
    {
        public static void DispatchThreads(this ComputeShader cs, int kernelIdx, int threadX, int threadY, int threadZ)
        {
            uint x, y, z;
            cs.GetKernelThreadGroupSizes(kernelIdx, out x, out y, out z);
            cs.Dispatch(kernelIdx, Mathf.CeilToInt(threadX / x), Mathf.CeilToInt(threadY / y), Mathf.CeilToInt(threadZ / z));

        }
    }

}