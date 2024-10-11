using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework.Editors
{
    internal static class UIBindGenerator
    {
        public delegate void CodeWriter(string indent, TextWriter writer);

        private const char BindableTag = '@';

        private const string BeginBindCodeTag = "#region Auto Generate UI Bind";
        private const string EndBindCodeTag = "#endregion Auto Generate UI Bind";

        private static readonly IBindableCodeGenerator[] CodeGenerators = new IBindableCodeGenerator[]
        {
            new BindButtonCodeGenerator(),
            new BindComponentCodeGenerator<Image>(),
            new BindComponentCodeGenerator<TextMeshProUGUI>(),
            new BindUIWidgetCodeGenerator(),
            new BindTransformCodeGenerator(),
            new BindGameObjectCodeGenerator(),
        };

        public static void GenerateBindCode(MonoBehaviour target)
        {
            var filePath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(target));
            ClearBindCode(filePath);

            var bindables = GetAllBindableNodes(target.gameObject);
            var targetType = target.GetType();

            var currentIndent = "";
            using var writer = new StringWriter(new StringBuilder(File.ReadAllText(filePath, Encoding.UTF8)));

            BeginGenerateScope(writer, ref currentIndent, targetType);
            {
                for (var index = 0; index < bindables.Length; ++index)
                {
                    ref var node = ref bindables[index];
                    CodeGenerators[node.BindableIndex].WriteMethodCode(writer, currentIndent, targetType, node);
                }

                writer.WriteLine();
                for (var index = 0; index < bindables.Length; ++index)
                {
                    ref var node = ref bindables[index];
                    writer.WriteLine($"{currentIndent}[UnityEngine.SerializeField]");
                    CodeGenerators[node.BindableIndex].WriteFieldCode(writer, currentIndent, targetType, node);
                }

                writer.WriteLine();
                writer.WriteLine($"{currentIndent}[UnityEngine.ContextMenu(\"Rebind Wdigets\"), System.Obsolete(\"Don't Call In Code.\", true)]");
                writer.WriteLine($"{currentIndent}private void RebindAllWidgets()");
                BeginScope(writer, ref currentIndent);
                {
                    writer.WriteLine("#if UNITY_EDITOR");
                    writer.WriteLine($"{currentIndent}UnityEditor.Undo.RecordObject(this, \"Bind All Widgets\");");
                    writer.WriteLine("#endif");
                    writer.WriteLine();

                    for (var index = 0; index < bindables.Length; ++index)
                    {
                        ref var node = ref bindables[index];
                        CodeGenerators[node.BindableIndex].WriteBindCode(writer, currentIndent, targetType, node);
                    }

                    writer.WriteLine();
                    writer.WriteLine("#if UNITY_EDITOR");
                    writer.WriteLine($"{currentIndent}UnityEditor.EditorUtility.SetDirty(this);");
                    writer.WriteLine("#endif");
                }
                EndScope(writer, ref currentIndent);

                writer.WriteLine();
                writer.WriteLine($"{currentIndent}[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]");
                writer.WriteLine($"{currentIndent}private void InitializeAllWidgets()");
                BeginScope(writer, ref currentIndent);
                {
                    for (var index = 0; index < bindables.Length; ++index)
                    {
                        ref var node = ref bindables[index];
                        CodeGenerators[node.BindableIndex].WriteInitializeCode(writer, currentIndent, targetType, node);
                    }
                }
                EndScope(writer, ref currentIndent);
            }
            EndGenerateScope(writer, ref currentIndent, targetType);

            File.WriteAllText(filePath, writer.ToString(), Encoding.UTF8);
        }

        private static void ClearBindCode(string filePath)
        {
            var sb = new StringBuilder();

            using var reader = new StreamReader(filePath, Encoding.UTF8);
            var inBindRegion = false;
            for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                inBindRegion = inBindRegion || line.IndexOf(BeginBindCodeTag) >= 0;
                if (!inBindRegion)
                {
                    sb.AppendLine(line);
                }
                inBindRegion = inBindRegion || line.IndexOf(EndBindCodeTag) >= 0;
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

        private static void BeginScope(TextWriter writer, ref string indent)
        {
            writer.WriteLine($"{indent}{{");
            indent += '\t';
        }

        private static void EndScope(TextWriter writer, ref string indent)
        {
            indent = indent.Substring(0, indent.Length - 1);
            writer.WriteLine($"{indent}}}");
        }

        private static void BeginGenerateScope(TextWriter writer, ref string indent, Type type)
        {
            writer.WriteLine("");
            writer.WriteLine(BeginBindCodeTag);
            writer.WriteLine("#pragma warning disable IDE0052, IDE0001");
            if (!string.IsNullOrEmpty(type.Namespace))
            {
                writer.WriteLine("namespace " + type.Namespace);
                BeginScope(writer, ref indent);
            }

            writer.WriteLine($"{indent}partial class {type.Name}");
            BeginScope(writer, ref indent);

            writer.WriteLine($"{indent}protected override void Awake()");
            BeginScope(writer, ref indent);
            {
                writer.WriteLine($"{indent}base.Awake();");
                writer.WriteLine($"{indent}InitializeAllWidgets();");
                writer.WriteLine($"{indent}OnAwake();");
            }
            EndScope(writer, ref indent);

            writer.WriteLine();
            writer.WriteLine($"{indent}partial void OnAwake();");
        }

        private static void EndGenerateScope(TextWriter writer, ref string indent, Type type)
        {
            EndScope(writer, ref indent);
            if (!string.IsNullOrEmpty(type.Namespace))
            {
                EndScope(writer, ref indent);
            }

            writer.WriteLine("#pragma warning restore IDE0052, IDE0001");
            writer.WriteLine(EndBindCodeTag);
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
                    var child = transform.GetChild(index);
                    if (!PrefabUtility.IsAnyPrefabInstanceRoot(child.gameObject) || child.TryGetComponent<PrefabDepthCodeGenerate>(out _))
                    {
                        stack.Push(transform.GetChild(index));
                    }
                }

                if (transform.name.StartsWith(BindableTag))
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
                    Target = gameObject,
                    FieldName = "m_" + gameObject.name[1..],
                    NodeName = gameObject.name[1..],
                    NodePath = AnimationUtility.CalculateTransformPath(gameObject.transform, target.transform),
                    BindableIndex = GetBindableIndex(gameObject)
                };
            }
            return bindableNodes;
        }

        private static int GetBindableIndex(GameObject target)
        {
            for (var index = 0; index < CodeGenerators.Length; ++index)
            {
                if (CodeGenerators[index].IsCanBind(target))
                {
                    return index;
                }
            }
            return -1;
        }
    }
}
