using System.IO;
using UnityEngine;

namespace UIFramework
{
    internal readonly struct BindableNode
    {
        public string Tag { get; init; }
        public GameObject Target { get; init; }
        public string FieldName { get; init; }
        public string NodeName { get; init; }
        public string NodePath { get; init; }
    }

    internal interface IBindableCodeGenerator
    {
        bool IsCanBind(GameObject target);
        void WriteFieldCode(StreamWriter writer, MonoBehaviour target, in BindableNode bindable, string indent);
        void WriteBindCode(StreamWriter writer, MonoBehaviour target, in BindableNode bindable, string indent);
    }
}
