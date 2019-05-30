using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;

namespace SyncUtil
{
    public static class LockStepHelper
    {
        public static string GenerateComputeBufferHash<T>(ComputeBuffer cb) where T: struct
        {
            var count = cb.count;
            var datas = new T[count];
            cb.GetData(datas);

            var size = Marshal.SizeOf(typeof(T));

            var ptr = Marshal.AllocHGlobal(size);
            var bytes = new byte[size * count];
            for (var i = 0; i < datas.Length; ++i)
            {
                Marshal.StructureToPtr(datas[i], ptr, false);
                Marshal.Copy(ptr, bytes, i * size, size);
            }
            Marshal.FreeHGlobal(ptr);

            var algorithm = HashAlgorithm.Create("SHA256");

            var hash = algorithm.ComputeHash(bytes);
            algorithm.Clear();

            return hash.Aggregate("", (result, b) => result + b.ToString("X2"));
        }
    }
}