using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace SyncUtil
{
    public class SyncParam : MonoBehaviour
    {
        public enum Mode
        {
            Sync,
            Trigger
        }

        [FormerlySerializedAs("_target")]
        public Object target;

        [FormerlySerializedAs("_fields")]
        public List<FieldData> fields = new List<FieldData>();

        public void Start()
        {
            fields.ForEach(field => field.Init(target));
        }

        void Update()
        {
            var mgr = SyncParamManager.instance;
            if (mgr != null)
            {
                if (SyncNet.isServer)
                {
                    fields.ForEach(field =>
                    {
                        mgr.UpdateParam(field.key, field.GetValue(target));
                    });
                }

                if (SyncNet.isSlave)
                {
                    fields.ForEach(field =>
                    {
                        var obj = mgr.GetParam(field.key, field.mode == Mode.Trigger);
                        if (obj != null)
                        {
                            field.SetValue(target, obj);
                        }
                    });
                }
            }
        }


        [System.Serializable]
        public class FieldData
        {
            public string name;
            public Mode mode;

            public string key { get; protected set; }
            FieldInfo fieldInfo;

            bool? needSerialize_;
            bool needSerialize => needSerialize_ ?? (needSerialize_ = !SyncParamManager.instance.IsTypeSupported(fieldInfo.FieldType)).Value;

            public void Init(Object target)
            {
                key = target.name + "/" + name; // TODO: case _target object has multi instance

                var t = target.GetType();
                while (t != null && fieldInfo == null)
                {
                    fieldInfo = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    t = t.BaseType;
                }
                Assert.IsNotNull(fieldInfo, "Can't get field. [" + target.GetType() + "." + name + "]");
            }

            public object GetValue(Object target)
            {
                var ret = fieldInfo.GetValue(target);
                if ( needSerialize )
                {
                    ret = JsonUtility.ToJson(ret);
                }

                return ret;
            }

            public void SetValue(Object target, object value)
            {
                if (needSerialize)
                {
                    value = JsonUtility.FromJson(value as string, fieldInfo.FieldType);
                }
                fieldInfo.SetValue(target, value);
            }
        }


    }
}