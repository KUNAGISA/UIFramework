using System;
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

        public void WriteFieldCode(TextWriter writer, string indent, Type type, in BindableNode bindable)
        {
            writer.WriteLine($"{indent}private {bindable.Target.transform.GetType().FullName} {bindable.FieldName} = null;");
        }

        public void WriteBindCode(TextWriter writer, string indent, Type type, in BindableNode bindable)
        {
            writer.WriteLine($"{indent}{bindable.FieldName} = transform.Find(\"{bindable.NodePath}\") as {bindable.Target.transform.GetType().FullName};");
        }

        public void WriteInitializeCode(TextWriter writer, string indent, Type type, in BindableNode bindable)
        {

        }
    }
}
