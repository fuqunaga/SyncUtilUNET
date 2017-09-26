using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SyncUtil.Example
{

    [RequireComponent(typeof(Camera))]
    public class LifeGame : MonoBehaviour
    {
        public static class COMMONPARAM
        {
            public const string WIDTH = "_Width";
            public const string HEIGHT = "_Height";
        }

        public static class CSPARAM
        {
            public const string KERNEL_STEP = "Step";
            public const string WRITE_BUF = "_WriteBuf";
            public const string READ_BUF = "_ReadBuf";

            public const string KERNEL_INPUT = "Input";
            public const string INPUT_POS = "_InputPos";
            public const string INPUT_RADIUS = "_InputRadius";
        }

        public static class SHADERPARAM
        {
            public const string BUF = "_Buf";
        }

        public struct Data
        {
            public int alive;
        }

        [Header("CS")]
        public ComputeShader _cs;

        public float _stepInterval = 0.1f;
        public float _initialAliveRate = 0.2f;

        float _interval = 0f;
        ComputeBuffer _writeBufs;
        ComputeBuffer _readBufs;

        public float _InputRadius = 10f;

        [Header("Render")]
        public Shader _shader;
        Material _mat;

        void Start()
        {
            _mat = new Material(_shader);
        }

        private void OnDestroy()
        {
            DestroyBufs();
            if (_mat != null) Destroy(_mat);
        }

        void DestroyBufs()
        {
            if (_readBufs != null) { _readBufs.Release(); _readBufs = null; }
            if (_writeBufs != null) { _writeBufs.Release(); _writeBufs = null; }
        }

        public class StepData
        {
            public bool isResize;
            public int width;
            public int height;
            public int randSeed;
            public bool isInputEnable;
            public Vector2 inputPos;
            public float deltaTime;
        }

        public void Step(StepData data)
        {
            if (data.isResize)
            {
                DoResize(data);
            }

            if (data.isInputEnable)
            {
                DoInput(data);
            }
            _interval -= data.deltaTime;

            if (_interval <= 0f)
            {
                DoStep();
                _interval = Mathf.Max(0f, _interval + _stepInterval);
            }
        }

        [Header("Reproducibility")]
        // for inspector(and copy for reproducibility)
        public int _width;
        public int _height;
        public int _seed;

        void DoResize(StepData data)
        {
            _width = _width <= 0 ? data.width : _width;
            _height = _height <= 0 ? data.height : _height;

            _cs.SetInt(COMMONPARAM.WIDTH, _width);
            _cs.SetInt(COMMONPARAM.HEIGHT, _height);

            DestroyBufs();

            _seed = (_seed <= 0) ? data.randSeed : _seed;
            var rand = new System.Random(_seed);
            var gridNum = _width * _height;
            var bufs = Enumerable.Range(0, 2).Select(_ => new ComputeBuffer(gridNum, Marshal.SizeOf(typeof(Data)))).ToArray();

            _readBufs = bufs[0];
            _writeBufs = bufs[1];

            _readBufs.SetData(Enumerable.Range(0, gridNum).Select(_ => new Data() { alive = (rand.NextDouble() < _initialAliveRate) ? 1 : 0 }).ToArray());
        }


        void DoInput(StepData data)
        {
            var kernel = _cs.FindKernel(CSPARAM.KERNEL_INPUT);
            _cs.SetVector(CSPARAM.INPUT_POS, data.inputPos);
            _cs.SetFloat(CSPARAM.INPUT_RADIUS, _InputRadius);
            _cs.SetBuffer(kernel, CSPARAM.WRITE_BUF, _readBufs);

            Dispatch(_cs, kernel, new Vector3(_width, _height, 1));
        }

        void DoStep()
        {
            var kernel = _cs.FindKernel(CSPARAM.KERNEL_STEP);

            _cs.SetBuffer(kernel, CSPARAM.READ_BUF, _readBufs);
            _cs.SetBuffer(kernel, CSPARAM.WRITE_BUF, _writeBufs);

            Dispatch(_cs, kernel, new Vector3(_width, _height, 1));

            SwapBuf();
        }

        void SwapBuf()
        {
            var tmp = _readBufs;
            _readBufs = _writeBufs;
            _writeBufs = tmp;
        }


        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_readBufs != null)
            {
                _mat.SetInt(COMMONPARAM.WIDTH, _width);
                _mat.SetInt(COMMONPARAM.HEIGHT, _height);
                _mat.SetBuffer(SHADERPARAM.BUF, _readBufs);
                Graphics.Blit(source, destination, _mat);
            }
        }

        public static void Dispatch(ComputeShader cs, int kernel, Vector3 threadNum)
        {
            uint x, y, z;
            cs.GetKernelThreadGroupSizes(kernel, out x, out y, out z);
            cs.Dispatch(kernel, Mathf.CeilToInt(threadNum.x / x), Mathf.CeilToInt(threadNum.y / y), Mathf.CeilToInt(threadNum.z / z));
        }

    }
}