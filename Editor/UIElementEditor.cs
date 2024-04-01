using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UIFramework
{
    [CustomEditor(typeof(UIElement), editorForChildClasses: true)]
    internal class UIElementEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var target = this.target as UIElement;
            var stage = PrefabStageUtility.GetPrefabStage(target.gameObject);
            if (stage == null || EditorApplication.isPlaying) 
            {
                return;
            }

            if (GUILayout.Button("生成绑定控件代码"))
            {
                UIBindGenerator.GenerateBindCode(target);
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("清除绑定控件代码"))
            {
                UIBindGenerator.ClearBindCode(target);
                AssetDatabase.Refresh();
            }
        }
    }
}
