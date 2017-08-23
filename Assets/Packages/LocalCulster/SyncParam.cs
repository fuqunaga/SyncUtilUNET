using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace LocalClustering
{
    public class SyncParam : MonoBehaviour
    {
        public enum Mode
        {
            Sync,
            Trigger
        }


        public Object _target;
        public List<FieldData> _fields = new List<FieldData>();

        public void Start()
        {
            _fields.ForEach(field => field.Init(_target));
        }

        void Update()
        {
            var mgr = SyncParamManager.instance;
            if (mgr != null)
            {
                if (LocalCluster.isServer)
                {
                    _fields.ForEach(field =>
                    {
                        mgr.UpdateParam(field.key, field.GetValue(_target));
                    });
                }

                if (LocalCluster.isSlaver)
                {
                    _fields.ForEach(field =>
                    {
                        var obj = mgr.GetParam(field.key, field.mode == Mode.Trigger);
                        if (obj != null)
                        {
                            field.SetValue(_target, obj);
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

            public string key { get { return _key; } }
            string _key;
            FieldInfo _fieldInfo;

            public void Init(Object target)
            {
                _key = target.name + "/" + name; // TODO: case _target object has multi instance

                var t = target.GetType();
                while (t != null && _fieldInfo == null)
                {
                    _fieldInfo = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    t = t.BaseType;
                }
                Assert.IsNotNull(_fieldInfo, "Can't get field. [" + target.GetType() + "." + name + "]");
            }

            public object GetValue(Object target) { return _fieldInfo.GetValue(target); }
            public void SetValue(Object target, object value) { _fieldInfo.SetValue(target, value); }
        }


    }
}