using System.IO;
using UnityEngine;

namespace UIFramework
{
    internal class BindTransformCodeGenerator : IBindableCodeGenerator
    {
        public bool IsCanBind(GameObject target)
        {
            return true;
        }

        public void WriteFieldCode(StreamWriter writer, MonoBehaviour target, in BindableNode bindable, string indent)
        {
            writer.Write(indent);
            writer.Write("private ");
            writer.Write(typeof(Transform).FullName);
            writer.Write(' ');
            writer.Write(bindable.FieldName);
            writer.WriteLine(" = null;");
        }

        public void WriteBindCode(StreamWriter writer, MonoBehaviour target, in BindableNode bindable, string indent)
        {
            writer.Write(indent);
            writer.Write(bindable.FieldName);
            writer.Write(" = ");
            writer.Write("transform.Find(\"");
            writer.Write(bindable.NodePath);
            writer.WriteLine("\");");
        }
    }
}
