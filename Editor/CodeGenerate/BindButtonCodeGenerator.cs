using System;
using System.IO;
using UnityEngine.UI;

namespace UIFramework
{
    internal class BindButtonCodeGenerator : BindComponentCodeGenerator<Button>
    {
        public override void WriteMethodCode(TextWriter writer, string indent, Type type, in BindableNode bindable)
        {
            base.WriteMethodCode(writer, indent, type, bindable);
            writer.WriteLine($"{indent}partial void On{bindable.NodeName}Click();");
        }

        public override void WriteInitializeCode(TextWriter writer, string indent, Type type, in BindableNode bindable)
        {
            base.WriteInitializeCode(writer, indent, type, bindable);
            writer.WriteLine($"{indent}{bindable.FieldName}.onClick.AddListener(() => On{bindable.NodeName}Click());");
        }
    }
}
