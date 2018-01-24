using UnityEngine;

namespace SyncUtil.Example
{

    public class SyncParamExample : MonoBehaviour
    {
        public bool _bool;
        public int _int;
        public uint _uint;
        public float _float;
        public string _string;
        public Vector2 _vector2;
        public Vector3 _vector3;
        public Vector4 _vector4;

        private void OnEnable()
        {
            DebugMenuForExample.Instance.onGUI += DebugMenu;
        }

        private void OnDisable()
        {
            DebugMenuForExample.Instance.onGUI -= DebugMenu;
        }

        public void DebugMenu()
        {
            GUILayout.Label("SyncParamExample");
            GUIUtil.Indent(() =>
            {
                _bool = GUIUtil.Field(_bool, "bool");
                _int = GUIUtil.Field(_int, "int");
                _uint = GUIUtil.Field(_uint, "uint");
                _float = GUIUtil.Field(_float, "float");
                _string = GUIUtil.Field(_string, "string");
                _vector2 = GUIUtil.Field(_vector2, "vector2");
                _vector3 = GUIUtil.Field(_vector3, "vector3");
                _vector4 = GUIUtil.Field(_vector4, "vector4");
            });
        }
    }
}
