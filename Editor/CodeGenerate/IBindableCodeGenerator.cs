using System;
using System.IO;
using UnityEngine;

namespace System.Runtime.CompilerServices
{
    internal class IsExternalInit { }
}

namespace UIFramework
{
    internal readonly struct BindableNode
    {
        public GameObject Target { get; init; }
        public string FieldName { get; init; }
        public string NodeName { get; init; }
        public string NodePath { get; init; }
        public int BindableIndex { get; init; }
    }

    internal interface IBindableCodeGenerator
    {
        bool IsCanBind(GameObject target);
        void WriteMethodCode(TextWriter writer, string indent, Type type, in BindableNode bindable);
        void WriteFieldCode(TextWriter writer, string indent, Type type, in BindableNode bindable);
        void WriteBindCode(TextWriter writer, string indent, Type type, in BindableNode bindable);
        void WriteInitializeCode(TextWriter writer, string indent, Type type, in BindableNode bindable);
    }
}
