using System;
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

        public void WriteFieldCode(TextWriter writer, string indent, Type type, in BindableNode bindable)
        {
            writer.WriteLine($"{indent}private {typeof(T).FullName} {bindable.FieldName} = null;");
        }

        public void WriteBindCode(TextWriter writer, string indent, Type type, in BindableNode bindable)
        {
            writer.WriteLine($"{indent}{bindable.FieldName} = transform.Find(\"{bindable.NodePath}\").GetComponent<{typeof(T).FullName}>();");
        }

        public virtual void WriteInitializeCode(TextWriter writer, string indent, Type type, in BindableNode bindable)
        {

        }
    }
}
