using System;
using System.IO;
using System.Reflection;
using UnityEngine.UI;

namespace UIFramework
{
    internal class BindButtonCodeGenerator : BindComponentCodeGenerator<Button>
    {
        public override void WriteInitializeCode(TextWriter writer, string indent, Type type, in BindableNode bindable)
        {
            base.WriteInitializeCode(writer, indent, type, bindable);

            var method = type.GetMethod($"On{bindable.NodeName}Click", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null)
            {
                writer.WriteLine($"{indent}{bindable.FieldName}.onClick.AddListener({method.Name});");
            }
            else if (bindable.NodeName == "Close" && type.GetMethod("CloseSelf", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null)
            {
                writer.WriteLine($"{indent}{bindable.FieldName}.onClick.AddListener(CloseSelf);");
            }
        }
    }
}
