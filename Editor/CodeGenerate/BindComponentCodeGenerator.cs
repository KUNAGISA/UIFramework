using System.IO;
using UnityEngine;

namespace UIFramework
{
    internal class BindComponentCodeGenerator<T> : IBindableCodeGenerator where T : Component
    {
        public bool IsCanBind(GameObject target)
        {
            return target.TryGetComponent<T>(out var _);
        }

        public virtual void WriteFieldCode(StreamWriter writer, MonoBehaviour target, in BindableNode bindable, string indent)
        {
            writer.Write(indent);
            writer.Write("private ");
            writer.Write(typeof(T).FullName);
            writer.Write(' ');
            writer.Write(bindable.FieldName);
            writer.WriteLine(" = null;");
        }

        public virtual void WriteBindCode(StreamWriter writer, MonoBehaviour target, in BindableNode bindable, string indent)
        {
            writer.Write(indent);
            writer.Write(bindable.FieldName);
            writer.Write(" = ");
            writer.Write("transform.Find(\"");
            writer.Write(bindable.NodePath);
            writer.Write("\").GetComponent<");
            writer.Write(typeof(T).FullName);
            writer.WriteLine(">();");
        }
    }
}
