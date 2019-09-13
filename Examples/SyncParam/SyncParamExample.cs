using System;
using UnityEngine;

namespace SyncUtil.Example
{
    public class SyncParamExample : MonoBehaviour
    {
        #region Type Define

        public enum MyEnum
        {
            One,
            Two,
            Three
        }

        [Serializable]
        public class MyClass
        {
            public int intVal;
            public float floatVal;
        }

        #endregion

        public MyEnum enumVal;
        public bool boolVal;
        public int intVal;
        public uint uintVal;
        public float floatVal;
        public string stringVal;
        public Vector2 vector2Val;
        public Vector3 vector3Val;
        public Vector4 vector4Val;

        public MyClass classVal;

        private void OnEnable()
        {
            DebugMenuForExample.Instance.onGUI += DebugMenu;
        }

        private void OnDisable()
        {
            var menu = DebugMenuForExample.Instance;
            if (menu != null)
            {
                menu.onGUI -= DebugMenu;
            }
        }

        public void DebugMenu()
        {
            GUILayout.Label("SyncParamExample");
            GUIUtil.Indent(() =>
            {
                enumVal = GUIUtil.Field(enumVal, nameof(enumVal));
                boolVal = GUIUtil.Field(boolVal, nameof(boolVal));
                intVal = GUIUtil.Field(intVal, nameof(intVal));
                uintVal = GUIUtil.Field(uintVal, nameof(uintVal));
                floatVal = GUIUtil.Field(floatVal, nameof(floatVal));
                stringVal = GUIUtil.Field(stringVal, nameof(stringVal));
                vector2Val = GUIUtil.Field(vector2Val, nameof(vector2Val));
                vector3Val = GUIUtil.Field(vector3Val, nameof(vector3Val));
                vector4Val = GUIUtil.Field(vector4Val, nameof(vector4Val));
                classVal.intVal = GUIUtil.Field(classVal.intVal, nameof(classVal) + ".intVal");
                classVal.floatVal = GUIUtil.Field(classVal.floatVal, nameof(classVal) + ".floatVal");
            });
        }
    }
}
