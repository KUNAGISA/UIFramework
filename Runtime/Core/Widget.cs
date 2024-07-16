using UnityEngine;

namespace UIFramework
{
    [DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
    public abstract class Widget : MonoBehaviour
    {
        [ContextMenu("Generate Bind Code", isValidateFunction: false, priority: 2000000)]
        private void GenerateBindCode() => CodeGenerator.OnGenerateBindCode?.Invoke(this);
    }
}
