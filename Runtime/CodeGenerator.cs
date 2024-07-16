using System;
using UnityEngine;

namespace UIFramework
{
    internal static class CodeGenerator
    {
        internal static Action<MonoBehaviour> OnGenerateBindCode { get; set; } = null;
    }
}
