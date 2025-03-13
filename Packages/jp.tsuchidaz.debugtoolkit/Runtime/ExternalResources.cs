#nullable enable

using UnityEngine;
using UnityEngine.UIElements;

namespace DebugToolkit
{
    internal sealed class ExternalResources : ScriptableObject
    {
        [SerializeField]
        private ThemeStyleSheet? _themeStyleSheet;

        public const string rootPath = "Packages/jp.tsuchidaz.debugtoolkit/External Resources";

#if !UNITY_EDITOR
        private static ExternalResources s_instance;

        private void OnEnable()
        {
            s_instance = this;
        }
#endif

        public static ThemeStyleSheet LoadThemeStyleSheet()
        {
#if UNITY_EDITOR
            
            return UnityEditor.AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(rootPath + "/DefaultRuntimeTheme.tss");
#else
            return s_instance._themeStyleSheet;
#endif
        }
    }
}