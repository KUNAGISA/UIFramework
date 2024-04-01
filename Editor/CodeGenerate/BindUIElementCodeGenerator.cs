using Gameplay;
using System.IO;
using UnityEngine;

namespace UIFramework
{
    internal class BindUIElementCodeGenerator : IBindableCodeGenerator
    {
        private string GetFullType(GameObject target)
        {
            var component = target.GetComponent<UIElement>();
            return component.GetType().FullName;
        }

        public bool IsCanBind(GameObject target)
        {
            return target.TryGetComponent<UIElement>(out var _);
        }

        public void WriteFieldCode(StreamWriter writer, MonoBehaviour target, in BindableNode bindable, string indent)
        {
            writer.Write(indent);
            writer.Write("private ");
            writer.Write(GetFullType(bindable.Target));
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
            writer.Write("\").GetComponent<");
            writer.Write(GetFullType(bindable.Target));
            writer.WriteLine(">();");
        }
    }
}
