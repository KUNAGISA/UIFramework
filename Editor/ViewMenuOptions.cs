using UnityEditor;
using UnityEngine;

namespace UIFramework.Editors
{
    [InitializeOnLoad]
    internal static class ViewMenuOptions
    {
        static ViewMenuOptions()
        {
            CodeGenerator.OnGenerateBindCode = GenerateBindCode;
        }

        private static void GenerateBindCode(MonoBehaviour behaviour)
        {
            UIBindGenerator.GenerateBindCode(behaviour);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            var script = MonoScript.FromMonoBehaviour(behaviour);
            AssetDatabase.OpenAsset(script);
        }

        [MenuItem("Assets/Create/Scripting/View Script", priority = 10000)]
        public static void CreateViewScriptFile()
        {
            const string TemplateFilePath = "./Assets/Scripts/UIFramework/Editor/Template/View.cs.txt";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(TemplateFilePath, "View.cs");
        }

        [MenuItem("Assets/Create/Scripting/Widget Script", priority = 10001)]
        public static void CreateWidgetScriptFile()
        {
            const string TemplateFilePath = "./Assets/Scripts/UIFramework/Editor/Template/Widget.cs.txt";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(TemplateFilePath, "Widget.cs");
        }
    }
}
