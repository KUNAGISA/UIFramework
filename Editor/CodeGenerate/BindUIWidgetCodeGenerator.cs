using System;
using System.IO;
using UnityEngine;

namespace UIFramework
{
    internal class BindUIWidgetCodeGenerator : IBindableCodeGenerator
    {
        private string GetFullType(GameObject target)
        {
            var component = target.GetComponent<Widget>();
            return component.GetType().FullName;
        }

        public bool IsCanBind(GameObject target)
        {
            return target.TryGetComponent<Widget>(out var _);
        }

        public void WriteFieldCode(TextWriter writer, string indent, Type type, in BindableNode bindable)
        {
            writer.WriteLine($"{indent}private {GetFullType(bindable.Target)} {bindable.FieldName} = null;");
        }

        public void WriteBindCode(TextWriter writer, string indent, Type type, in BindableNode bindable)
        {
            writer.WriteLine($"{indent}{bindable.FieldName} = transform.Find(\"{bindable.NodePath}\").GetComponent<{GetFullType(bindable.Target)}>();");
        }

        public void WriteInitializeCode(TextWriter writer, string indent, Type type, in BindableNode bindable)
        {

        }
    }
}
