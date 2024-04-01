using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    internal static class UIBindGenerator
    {
        private static readonly Regex BindableRegex = new Regex(@"^([a-z]+)_(.+)");

        private const string BeginBindCode = "#region Auto Generate UI Bind";
        private const string EndBindCode = "#endregion Auto Generate UI Bind";

        private static readonly Dictionary<string, IBindableCodeGenerator> CodeGenerators = new Dictionary<string, IBindableCodeGenerator>()
        {
            {"btn", new BindButtonCodeGenerator() },
            {"img", new BindComponentCodeGenerator<Image>() },
            {"txt", new BindComponentCodeGenerator<TextMeshProUGUI>() },
            {"ui", new BindUIElementCodeGenerator() },
            {"node", new BindTransformCodeGenerator() },
            {"obj", new BindGameObjectCodeGenerator() },
        };

        public static void ClearBindCode(MonoBehaviour target)
        {
            var filePath = GetMonoBehaviourFilePath(target);
            ClearBindCode(filePath);
        }

        public static void ClearBindCode(string filePath)
        {
            var sb = new StringBuilder();

            using var reader = new StreamReader(filePath, Encoding.UTF8);
            var inBindRegion = false;
            for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                inBindRegion = inBindRegion || line.IndexOf(BeginBindCode) >= 0;
                if (!inBindRegion)
                {
                    sb.AppendLine(line);
                }
                inBindRegion = inBindRegion || line.IndexOf(EndBindCode) >= 0;
            }
            reader.Close();

            int index = sb.Length - 1;
            while (index >= 0 && (sb[index] == '\r' || sb[index] == '\n' || char.IsWhiteSpace(sb[index])))
            {
                index--;
            }

            if (index < sb.Length - 1)
            {
                sb.Length = index + 1;
            }

            sb.AppendLine();
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        public static void GenerateBindCode(MonoBehaviour target)
        {
            var filePath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(target));
            ClearBindCode(filePath);

            var bindables = GetAllBindableNodes(target.gameObject);
            var targetType = target.GetType();

            using var writer = new StreamWriter(filePath, true, Encoding.UTF8);

            writer.WriteLine("");
            writer.WriteLine(BeginBindCode);
            writer.WriteLine("namespace " + targetType.Namespace);
            writer.WriteLine("{");

            writer.Write("\t");
            writer.Write(targetType.IsPublic ? "public" : "internal");
            writer.Write(" partial class ");
            writer.Write(targetType.Name);
            writer.Write(" : ");
            writer.WriteLine(targetType.BaseType.FullName);
            writer.WriteLine("\t{");

            for(var index =  0; index < bindables.Length; ++index)
            {
                ref var node = ref bindables[index];
                CodeGenerators[node.Tag].WriteFieldCode(writer, target, node, "\t\t");
            }

            writer.WriteLine();
            writer.WriteLine("\t\t[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]");
            writer.WriteLine("\t\tprivate void BindAllWidgets()");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\tvar transform = this.transform;");

            for (var index = 0; index < bindables.Length; ++index)
            {
                writer.WriteLine();
                ref var node = ref bindables[index];
                CodeGenerators[node.Tag].WriteBindCode(writer, target, node, "\t\t\t");
            }

            writer.WriteLine("\t\t}");

            writer.WriteLine("\t}");
            writer.WriteLine("}");
            writer.Write(EndBindCode);

            writer.Close();
        }

        private static BindableNode[] GetAllBindableNodes(GameObject target)
        {
            var stack = new Stack<Transform>(20);
            stack.Push(target.transform);

            var bindableGameObjects = new List<GameObject>(20);

            while (stack.TryPop(out var transform))
            {
                for (var index = 0; index < transform.childCount; ++index)
                {
                    stack.Push(transform.GetChild(index));
                }

                var tag = GetBindableTag(transform.name) ?? string.Empty;
                if (CodeGenerators.TryGetValue(tag, out var generator) && generator.IsCanBind(transform.gameObject))
                {
                    bindableGameObjects.Add(transform.gameObject);
                }
            }

            var bindableNodes = new BindableNode[bindableGameObjects.Count];
            for (var index = 0; index < bindableGameObjects.Count; ++index)
            {
                var gameObject = bindableGameObjects[index];
                bindableNodes[index] = new BindableNode()
                {
                    Tag = GetBindableTag(gameObject.name),
                    Target = gameObject,
                    FieldName = GetBindableFieldName(gameObject.name),
                    NodeName = GetBindableNodeName(gameObject.name),
                    NodePath = AnimationUtility.CalculateTransformPath(gameObject.transform, target.transform)
                };
            }
            return bindableNodes;
        }

        private static string GetMonoBehaviourFilePath(MonoBehaviour target)
        {
            return AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(target));
        }

        private static string GetBindableTag(string name)
        {
            var result = BindableRegex.Match(name);
            return result.Success ? result.Groups[1].Value : string.Empty;
        }

        private static string GetBindableNodeName(string name)
        {
            var result = BindableRegex.Match(name);
            name = result.Success ? result.Groups[2].Value : name;
            return char.ToUpper(name[0]) + name[1..];
        }

        private static string GetBindableFieldName(string name)
        {
            var result = BindableRegex.Match(name);
            name = result.Success ? result.Groups[2].Value : name;
            return "m_" + char.ToLower(name[0]) + name[1..];
        }
    }
}
