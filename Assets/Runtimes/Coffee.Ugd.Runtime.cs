using UnityEngine;

namespace Coffee.Ugd
{
    public class Runtime
    {
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            Debug.Log(">>>> [Coffee.Ugd.Runtime] InitializeOnLoadMethod");
            var v = new Vector3(1, 20, 300);
            System.IO.File.WriteAllText("success_compile", string.Format("{0}, {1}, {2}", Math.GetX(v), Math.GetY(v), Math.GetZ(v)));
        }

        private static void Execute()
        {
            Debug.Log(">>>> [Coffee.Ugd.Runtime] Execute");
            var v = new Vector3(1, 20, 300);
            System.IO.File.WriteAllText("success_execute", string.Format("{0}, {1}, {2}", Math.GetX(v), Math.GetY(v), Math.GetZ(v)));
        }
#endif

        public void GetX()
        {
            var v = new Vector3(1, 20, 300);
            Debug.Log(Math.GetX(v));
        }

        public void GetY()
        {
            var v = new Vector3(1, 20, 300);
            Debug.Log(Math.GetY(v));
        }

        public void GetZ()
        {
            var v = new Vector3(1, 20, 300);
            Debug.Log(Math.GetZ(v));
        }
    }
}
