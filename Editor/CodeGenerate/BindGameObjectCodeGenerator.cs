using System;
using System.IO;
using UnityEngine;

namespace UIFramework
{
    internal class BindGameObjectCodeGenerator : IBindableCodeGenerator
    {
        public bool IsCanBind(GameObject target)
        {
            return true;
        }

        public void WriteFieldCode(TextWriter writer, string indent, Type type, in BindableNode bindable)
        {
            writer.WriteLine($"{indent}private {typeof(GameObject).FullName} {bindable.FieldName} = null;");
        }

        public void WriteBindCode(TextWriter writer, string indent, Type type, in BindableNode bindable)
        {
            writer.WriteLine($"{indent}{bindable.FieldName} = transform.Find(\"{bindable.NodePath}\").gameObject;");
        }

        public void WriteInitializeCode(TextWriter writer, string indent, Type type, in BindableNode bindable)
        {

        }

        public void WriteMethodCode(TextWriter writer, string indent, Type type, in BindableNode bindable)
        {

        }
    }
}
