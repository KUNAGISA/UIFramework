using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    internal class BindButtonCodeGenerator : BindComponentCodeGenerator<Button>
    {
        public override void WriteBindCode(StreamWriter writer, MonoBehaviour target, in BindableNode bindable, string indent)
        {
            base.WriteBindCode(writer, target, bindable, indent);

            var bindMethodName = $"On{bindable.NodeName}Click";
            if (target.GetType().GetMethod(bindMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null)
            {
                writer.Write(indent);
                writer.Write(bindable.FieldName);
                writer.Write(".onClick.AddListener(");
                writer.Write(bindMethodName);
                writer.WriteLine(");");
            }
            else if (bindable.NodeName == "Close" && target.GetType().GetMethod("CloseSelf", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null)
            {
                writer.Write(indent);
                writer.Write(bindable.FieldName);
                writer.WriteLine(".onClick.AddListener(CloseSelf);");
            }
        }
    }
}
