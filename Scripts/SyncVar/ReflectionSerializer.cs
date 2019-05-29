using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


namespace SyncUtil
{
    public static class ReflectionSerializer
    {
#if USE_REFLECTION
        static Dictionary<Type, List<FieldInfo>> typeDic = new Dictionary<Type, List<FieldInfo>>();

        static List<FieldInfo> GetFieldInfo(Type t)
        {
            if ( !typeDic.TryGetValue(t, out var ret))
            {
                t.GetFi
            }
        }

        public static void Serialize<T>(NetworkWriter write, T value)
        {
        }
#else
        static BinaryFormatter formatter = new BinaryFormatter();
        static MemoryStream stream = new MemoryStream();

        public static void Serialize<T>(NetworkWriter writer, T value)
        {
            stream.Position = 0;
            formatter.Serialize(stream, value);
            writer.WriteBytesAndSize(stream.GetBuffer());
        }

        public static T Deserialize<T>(NetworkReader reader)
        {
            var bytes = reader.ReadBytesAndSize();
            stream.Position = 0;
            stream.Write(bytes, 0, bytes.Length);

            return (T)formatter.Deserialize(stream);
        }
#endif
    }
}